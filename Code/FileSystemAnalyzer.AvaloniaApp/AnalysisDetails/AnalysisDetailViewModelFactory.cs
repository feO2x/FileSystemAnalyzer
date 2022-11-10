using System;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Explorer;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Folders;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

// ReSharper disable once ClassNeverInstantiated.Global -- class is instantiated by DI container
public sealed class AnalysisDetailViewModelFactory
{
    public AnalysisDetailViewModelFactory(Func<FileSystemAnalyzer> resolveFileSystemAnalyzer,
                                          Func<IFilesSession> createFilesSession,
                                          Func<IFoldersSession> createFoldersSession,
                                          Func<IExplorerSession> createExplorerSession,
                                          DebouncedValueFactory debouncedValueFactory,
                                          INavigateToAnalysesListCommand navigateCommand,
                                          ILogger logger)
    {
        ResolveFileSystemAnalyzer = resolveFileSystemAnalyzer;
        CreateFilesSession = createFilesSession;
        CreateFoldersSession = createFoldersSession;
        CreateExplorerSession = createExplorerSession;
        DebouncedValueFactory = debouncedValueFactory;
        NavigateCommand = navigateCommand;
        Logger = logger;
    }

    private Func<FileSystemAnalyzer> ResolveFileSystemAnalyzer { get; }
    private Func<IFilesSession> CreateFilesSession { get; }
    private Func<IFoldersSession> CreateFoldersSession { get; }
    private Func<IExplorerSession> CreateExplorerSession { get; }
    private DebouncedValueFactory DebouncedValueFactory { get; }
    private INavigateToAnalysesListCommand NavigateCommand { get; }
    private ILogger Logger { get; }

    public async Task<AnalysisDetailViewModel> CreateForNewAnalysisAsync(string directoryPath)
    {
        var fileSystemAnalyzer = ResolveFileSystemAnalyzer();
        var analysis = await fileSystemAnalyzer.CreateAnalysisAsync(directoryPath);
        return await CreateViewAsync(analysis, fileSystemAnalyzer);
    }

    public Task<AnalysisDetailViewModel> CreateForExistingAnalysisAsync(Analysis analysis) => CreateViewAsync(analysis);

    private async Task<AnalysisDetailViewModel> CreateViewAsync(Analysis analysis, FileSystemAnalyzer? fileSystemAnalyzer = null)
    {
        var filesViewModel = new FilesViewModel(analysis.Id, CreateFilesSession, DebouncedValueFactory, Logger);
        var foldersViewModel = new FoldersViewModel(analysis.Id, CreateFoldersSession, DebouncedValueFactory, Logger);
        var explorerViewModel = new ExplorerViewModel(analysis.Id, CreateExplorerSession, DebouncedValueFactory, Logger);
        var analysisDetailsViewModel = new AnalysisDetailViewModel(analysis, filesViewModel, foldersViewModel, explorerViewModel, fileSystemAnalyzer, NavigateCommand, Logger);

        if (fileSystemAnalyzer is null)
            await explorerViewModel.LoadFolderNodesAsync();
        else
            analysisDetailsViewModel.StartAnalysis();

        return analysisDetailsViewModel;
    }
}