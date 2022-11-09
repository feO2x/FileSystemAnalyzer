using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Light.GuardClauses;
using Serilog;
using Synnotech.Time;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

public sealed class FileSystemAnalyzer : IFileSystemAnalyzer
{
    public FileSystemAnalyzer(Func<IFileSystemAnalysisSession> createSession, IClock clock, ILogger logger)
    {
        CreateSession = createSession;
        Clock = clock;
        Logger = logger;
    }

    private Func<IFileSystemAnalysisSession> CreateSession { get; }
    private IClock Clock { get; }
    private ILogger Logger { get; }

    public Task AnalyzeFileSystemOnBackgroundThreadAsync(Analysis analysis,
                                                         IProgress<ProgressState> progress,
                                                         CancellationToken cancellationToken = default) =>
        Task.Run(() => AnalyzeFileSystemAsync(analysis, progress, cancellationToken), cancellationToken);

    public async Task<Analysis> CreateAnalysisAsync(string directoryPath,
                                                    CancellationToken cancellationToken = default)
    {
        var now = Clock.GetTime();
        var analysis = Analysis.CreateFromDirectoryPath(directoryPath, now);
        await using var session = CreateSession();
        await session.StoreAsync(analysis, cancellationToken);
        await session.SaveChangesAsync(cancellationToken);
        return analysis;
    }

    public async Task AnalyzeFileSystemAsync(Analysis analysis,
                                             IProgress<ProgressState> progress,
                                             CancellationToken cancellationToken = default)
    {
        analysis.Id.MustNotBeNullOrWhiteSpace(nameof(analysis), "The ID on the analysis entity must already be set");

        Logger.Debug("Creating tree for {@Analysis}...", analysis);
        var currentDirectory = new DirectoryInfo(analysis.DirectoryPath);

        // We use a non-recursive BFS approach here to avoid stack overflows in
        // scenarios where we have a high level of sub folder nesting.
        var queue = new Queue<DirectoryQueueEntry>();
        queue.Enqueue(new (currentDirectory, null));

        var progressManager = new ProgressManager(progress);
        try
        {
            await using var session = CreateSession();
            var inMemoryFileEntries = new List<FileSystemEntry>(1_000);
            while (queue.Count > 0)
            {
                var stackEntry = queue.Dequeue();

                // First, we create an entry for the current folder and append all files as child nodes at the same time
                var newFolderEntry = await CreateEntriesForFolderAndFiles(stackEntry, analysis, session, inMemoryFileEntries, progressManager, cancellationToken);

                // We then enqueue all subdirectories to continue the BFS in the next loop run
                EnqueueAllSubDirectories(stackEntry.DirectoryInfo, newFolderEntry.Id, queue);

                // Afterwards, we add the new folder entry to its parent folder if necessary
                // and update the size of all other parents until we reach the root of the tree
                await UpdateParentFoldersAsync(newFolderEntry, session, cancellationToken);
            }

            // Once we ran through the whole tree, we will update the analysis entity with the complete size
            await UpdateAnalysisAsync(analysis, progressManager.NumberOfProcessedFolders, progressManager.NumberOfProcessedFiles, session, cancellationToken);
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Could not analyze the directory \"{DirectoryPath}\" successfully", analysis.DirectoryPath);
            analysis.ErrorMessage = exception.ToString();
            await using var session = CreateSession();
            await session.StoreAsync(analysis, cancellationToken);
            await session.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            progressManager.ReportFinish();
        }
    }

    private async Task<FileSystemEntry> CreateEntriesForFolderAndFiles(DirectoryQueueEntry queueEntry,
                                                                       Analysis analysis,
                                                                       IFileSystemAnalysisSession session,
                                                                       List<FileSystemEntry> inMemoryFileEntries,
                                                                       ProgressManager progressManager,
                                                                       CancellationToken cancellationToken)
    {
        // First we create a folder entry from the current directory and store it in the database
        var directoryInfo = queueEntry.DirectoryInfo;
        var folderEntry = FileSystemEntry.CreateFolderEntry(directoryInfo.FullName,
                                                            analysis.Id,
                                                            queueEntry.ParentId);
        await session.StoreAsync(folderEntry, cancellationToken);
        Logger.Debug("Created entry for folder {@FolderEntry}", folderEntry);

        // We then check if this folder entry is the root -
        // if yes, then we will attach it to the analysis entity.
        if (folderEntry.ParentId is null)
        {
            analysis.RootEntryId = folderEntry.Id;
            await session.StoreAsync(analysis, cancellationToken);
            Logger.Debug("Attached folder entry {FolderEntryId} as root to analysis {AnalysisId}", folderEntry.Id, analysis.Id);
        }

        // Afterwards we enumerate all files and add them as children.
        foreach (var fileInfo in directoryInfo.EnumerateFiles())
        {
            var fileEntry = FileSystemEntry.FromFileInfo(fileInfo, analysis.Id, folderEntry.Id);
            await session.StoreAsync(fileEntry, cancellationToken);
            folderEntry.AddChildAndUpdateSize(fileEntry);

            inMemoryFileEntries.Add(fileEntry);
            if (inMemoryFileEntries.Count == inMemoryFileEntries.Capacity)
                await SaveChangesAndEvictFilesAsync(session, inMemoryFileEntries, cancellationToken);

            progressManager.ReportNewFile();
            Logger.Debug("Created file entry {@FileEntry} as child of {FolderEntryId}", fileEntry, folderEntry.Id);
        }

        // Once we are done, we will commit the transaction to store everything in the database.
        await SaveChangesAndEvictFilesAsync(session, inMemoryFileEntries, cancellationToken);
        Logger.Debug("Folder {FolderPath} has {NumberOfFiles} files with a total size of {SizeInBytes} Bytes",
                     folderEntry.FullPath,
                     folderEntry.ChildIds?.Count ?? 0,
                     folderEntry.SizeInBytes);
        progressManager.ReportNewFolder(folderEntry);
        return folderEntry;

        static async Task SaveChangesAndEvictFilesAsync(IFileSystemAnalysisSession session, List<FileSystemEntry> inMemoryFileEntries, CancellationToken cancellationToken)
        {
            await session.SaveChangesAsync(cancellationToken);
            session.EvictFileSystemEntries(inMemoryFileEntries);
            inMemoryFileEntries.Clear();
        }
    }

    private static void EnqueueAllSubDirectories(DirectoryInfo parentDirectory,
                                                 string parentId,
                                                 Queue<DirectoryQueueEntry> queue)
    {
        foreach (var subDirectory in parentDirectory.EnumerateDirectories())
        {
            queue.Enqueue(new (subDirectory, parentId));
        }
    }

    private async Task UpdateParentFoldersAsync(FileSystemEntry newFolderEntry, IFileSystemAnalysisSession session, CancellationToken cancellationToken)
    {
        if (newFolderEntry.ParentId is null)
            return;

        var currentParent = await session.GetFileSystemEntryAsync(newFolderEntry.ParentId, cancellationToken);
        currentParent.AddChildAndUpdateSize(newFolderEntry);
        Logger.Debug("Added folder {FolderId} as child to {ParentId} (new size {SizeInBytes} Bytes)", newFolderEntry.Id, currentParent.Id, currentParent.SizeInBytes);

        while (currentParent.ParentId is not null)
        {
            currentParent = await session.GetFileSystemEntryAsync(currentParent.ParentId, cancellationToken);
            currentParent.SizeInBytes += newFolderEntry.SizeInBytes;
            Logger.Debug("Updated size of parent {ParentId} to {SizeInBytes} Bytes", currentParent.Id, currentParent.SizeInBytes);
        }

        await session.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateAnalysisAsync(Analysis analysis,
                                           long numberOfFolders,
                                           long numberOfFiles,
                                           IFileSystemAnalysisSession session,
                                           CancellationToken cancellationToken)
    {
        var rootEntry = await session.GetFileSystemEntryAsync(analysis.RootEntryId, cancellationToken);
        analysis.SetRootEntry(rootEntry);
        analysis.NumberOfFolders = numberOfFolders;
        analysis.NumberOfFiles = numberOfFiles;
        analysis.FinishedAtUtc = Clock.GetTime();
        await session.StoreAsync(analysis, cancellationToken);
        await session.SaveChangesAsync(cancellationToken);
        Logger.Information("{@Analysis} was completed successfully", analysis);
    }
}