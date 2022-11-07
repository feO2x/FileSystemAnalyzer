using System;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

// ReSharper disable once ClassNeverInstantiated.Global -- class is instantiated by DI container
public sealed class AnalysisDetailViewFactory
{
    public AnalysisDetailViewFactory(Func<FileSystemAnalyzer> resolveFileSystemAnalyzer,
                                     INavigateToAnalysesListCommand navigateCommand,
                                     ILogger logger)
    {
        ResolveFileSystemAnalyzer = resolveFileSystemAnalyzer;
        NavigateCommand = navigateCommand;
        Logger = logger;
    }

    private Func<FileSystemAnalyzer> ResolveFileSystemAnalyzer { get; }
    private INavigateToAnalysesListCommand NavigateCommand { get; }
    private ILogger Logger { get; }

    public async Task<AnalysisDetailView> CreateForNewAnalysisAsync(string directoryPath)
    {
        var fileSystemAnalysis = ResolveFileSystemAnalyzer();
        var analysis = await fileSystemAnalysis.CreateAnalysisAsync(directoryPath);
        var analysisViewModel = new AnalysisDetailViewModel(analysis, fileSystemAnalysis, NavigateCommand, Logger);
        return new () { DataContext = analysisViewModel };
    }

    public AnalysisDetailView CreateForExistingAnalysis(Analysis analysis)
    {
        var analysisViewModel = new AnalysisDetailViewModel(analysis, null, NavigateCommand, Logger);
        return new () { DataContext = analysisViewModel };
    }
}