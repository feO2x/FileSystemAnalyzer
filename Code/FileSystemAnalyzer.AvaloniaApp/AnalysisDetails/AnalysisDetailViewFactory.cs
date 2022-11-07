using System;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

public sealed class AnalysisDetailViewFactory
{
    public AnalysisDetailViewFactory(Func<FileSystemAnalyzer> resolveFileSystemAnalyzer,
                                     ILogger logger)
    {
        ResolveFileSystemAnalyzer = resolveFileSystemAnalyzer;
        Logger = logger;
    }

    private Func<FileSystemAnalyzer> ResolveFileSystemAnalyzer { get; }
    private ILogger Logger { get; }

    public async Task<AnalysisDetailView> CreateForNewAnalysisAsync(string directoryPath)
    {
        var fileSystemAnalysis = ResolveFileSystemAnalyzer();
        var analysis = await fileSystemAnalysis.CreateAnalysisAsync(directoryPath);
        var analysisViewModel = new AnalysisDetailViewModel(analysis, fileSystemAnalysis, Logger);
        return new () { DataContext = analysisViewModel };
    }

    public AnalysisDetailView CreateForExistingAnalysis(Analysis analysis)
    {
        var analysisViewModel = new AnalysisDetailViewModel(analysis, null, Logger);
        return new () { DataContext = analysisViewModel };
    }
}