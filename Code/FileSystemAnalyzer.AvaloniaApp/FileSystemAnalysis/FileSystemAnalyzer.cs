using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Light.GuardClauses;
using Serilog;
using Synnotech.Time;

namespace FileSystemAnalyzer.AvaloniaApp.FileSystemAnalysis;

public sealed class FileSystemAnalyzer
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

    public Task AnalyzeFileSystemOnBackgroundThreadAsync(Analysis analysis,
                                                         IProgress<ProgressState> progress,
                                                         CancellationToken cancellationToken = default) =>
        Task.Run(() => AnalyzeFileSystemAsync(analysis, progress, cancellationToken), cancellationToken);

    public async Task AnalyzeFileSystemAsync(Analysis analysis,
                                             IProgress<ProgressState> progress,
                                             CancellationToken cancellationToken = default)
    {
        analysis.Id.MustNotBeNullOrWhiteSpace(nameof(analysis), "The ID on the analysis entity must already be set");

        Logger.Debug("Creating tree for {@Analysis}...", analysis);
        var currentDirectory = new DirectoryInfo(analysis.DirectoryPath);

        // We use a non-recursive DFS approach here to avoid stack overflows in
        // scenarios where we have a high level of sub folder nesting.
        var stack = new Stack<DirectoryStackEntry>();
        stack.Push(new (currentDirectory, null));

        var progressManager = new ProgressManager(progress);
        try
        {
            while (stack.Count > 0)
            {
                var stackEntry = stack.Pop();

                // First, we create an entry for the current folder and append all files as child nodes at the same time
                var newFolderEntry = await CreateEntriesForFolderAndFiles(stackEntry, analysis, progressManager, cancellationToken);

                // We then push all subdirectories to the stack to continue the DFS in the next loop run
                PushAllSubDirectories(stackEntry.DirectoryInfo, newFolderEntry.Id, stack);

                // Afterwards, we add the new folder entry to its parent folder if necessary
                // and update the size of all other parents until we reach the root of the tree
                await UpdateParentFoldersAsync(newFolderEntry, cancellationToken);
            }

            // Once we ran through the whole tree, we will update the analysis with the complete size
            await UpdateAnalysisAsync(analysis, progressManager.NumberOfProcessedFolders, progressManager.NumberOfProcessedFiles, cancellationToken);
            progressManager.ReportFinish();
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Could not analyze the directory \"{DirectoryPath}\" successfully", analysis.DirectoryPath);
            analysis.ErrorMessage = exception.ToString();
            await using var session = CreateSession();
            await session.StoreAsync(analysis, cancellationToken);
            await session.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<FileSystemEntry> CreateEntriesForFolderAndFiles(DirectoryStackEntry stackEntry,
                                                                       Analysis analysis,
                                                                       ProgressManager progressManager,
                                                                       CancellationToken cancellationToken)
    {
        await using var session = CreateSession();

        // First we create a folder entry from the current directory and store it in the database
        var directoryInfo = stackEntry.DirectoryInfo;
        var folderEntry = FileSystemEntry.CreateFolderEntry(directoryInfo.FullName,
                                                            analysis.Id,
                                                            stackEntry.ParentId);
        await session.StoreAsync(folderEntry, cancellationToken);
        progressManager.ReportNewFolder();
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
            progressManager.ReportNewFile();
            Logger.Debug("Created file entry {@FileEntry} as child of {FolderEntryId}", fileEntry, folderEntry.Id);
        }

        // Once we are done, we will commit the transaction to store everything in the database.
        await session.SaveChangesAsync(cancellationToken);
        Logger.Debug("Folder {FolderPath} has {NumberOfFiles} files with a total size of {SizeInBytes} Bytes",
                     folderEntry.FullPath,
                     folderEntry.ChildIds?.Count ?? 0,
                     folderEntry.SizeInBytes);
        return folderEntry;
    }

    private static void PushAllSubDirectories(DirectoryInfo parentDirectory,
                                              string parentId,
                                              Stack<DirectoryStackEntry> stack)
    {
        foreach (var subDirectory in parentDirectory.EnumerateDirectories())
        {
            stack.Push(new (subDirectory, parentId));
        }
    }

    private async Task UpdateParentFoldersAsync(FileSystemEntry newFolderEntry, CancellationToken cancellationToken)
    {
        if (newFolderEntry.ParentId is null)
            return;

        await using var session = CreateSession();
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
                                           CancellationToken cancellationToken)
    {
        await using var session = CreateSession();
        var rootEntry = await session.GetFileSystemEntryAsync(analysis.RootEntryId, cancellationToken);
        analysis.SizeInBytes = rootEntry.SizeInBytes;
        analysis.NumberOfFolders = numberOfFolders;
        analysis.NumberOfFiles = numberOfFiles;
        await session.StoreAsync(analysis, cancellationToken);
        await session.SaveChangesAsync(cancellationToken);
        Logger.Information("{@Analysis} was completed successfully", analysis);
    }
}