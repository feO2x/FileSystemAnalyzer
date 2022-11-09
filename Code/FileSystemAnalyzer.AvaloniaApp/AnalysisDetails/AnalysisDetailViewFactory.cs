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
public sealed class AnalysisDetailViewFactory
{
    public AnalysisDetailViewFactory(Func<FileSystemAnalyzer> resolveFileSystemAnalyzer,
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

    public async Task<AnalysisDetailView> CreateForNewAnalysisAsync(string directoryPath)
    {
        var fileSystemAnalyzer = ResolveFileSystemAnalyzer();
        var analysis = await fileSystemAnalyzer.CreateAnalysisAsync(directoryPath);
        return CreateView(analysis, fileSystemAnalyzer);
    }

    public AnalysisDetailView CreateForExistingAnalysis(Analysis analysis) => CreateView(analysis);

    private AnalysisDetailView CreateView(Analysis analysis, FileSystemAnalyzer? fileSystemAnalyzer = null)
    {
        var filesViewModel = new FilesViewModel(analysis.Id, CreateFilesSession, DebouncedValueFactory, Logger);
        var foldersViewModel = new FoldersViewModel(analysis.Id, CreateFoldersSession, DebouncedValueFactory, Logger);
        var explorerViewModel = new ExplorerViewModel(analysis.Id, CreateExplorerSession, DebouncedValueFactory, Logger);
        var analysisViewModel = new AnalysisDetailViewModel(analysis, filesViewModel, foldersViewModel, explorerViewModel, fileSystemAnalyzer, NavigateCommand, Logger);

        if (fileSystemAnalyzer is null)
            explorerViewModel.LoadFolderNodesAsync();
        
        return new () { DataContext = analysisViewModel };
    }
}