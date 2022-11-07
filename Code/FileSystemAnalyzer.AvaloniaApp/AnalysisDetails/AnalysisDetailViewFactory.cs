using System;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

// ReSharper disable once ClassNeverInstantiated.Global -- class is instantiated by DI container
public sealed class AnalysisDetailViewFactory
{
    public AnalysisDetailViewFactory(Func<FileSystemAnalyzer> resolveFileSystemAnalyzer,
                                     Func<IFilesSession> createFilesSession,
                                     DebouncedValueFactory debouncedValueFactory,
                                     INavigateToAnalysesListCommand navigateCommand,
                                     ILogger logger)
    {
        ResolveFileSystemAnalyzer = resolveFileSystemAnalyzer;
        CreateFilesSession = createFilesSession;
        DebouncedValueFactory = debouncedValueFactory;
        NavigateCommand = navigateCommand;
        Logger = logger;
    }

    private Func<FileSystemAnalyzer> ResolveFileSystemAnalyzer { get; }
    private Func<IFilesSession> CreateFilesSession { get; }
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
        var filesViewModel = new FilesViewModel(analysis, CreateFilesSession, DebouncedValueFactory, Logger);
        var analysisViewModel = new AnalysisDetailViewModel(analysis, filesViewModel, fileSystemAnalyzer, NavigateCommand, Logger);
        return new () { DataContext = analysisViewModel };
    }
}