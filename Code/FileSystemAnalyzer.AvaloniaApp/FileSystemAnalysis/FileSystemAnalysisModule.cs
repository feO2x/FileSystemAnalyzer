using LightInject;

namespace FileSystemAnalyzer.AvaloniaApp.FileSystemAnalysis;

public static class FileSystemAnalysisModule
{
    public static IServiceRegistry RegisterFileSystemAnalysis(this IServiceRegistry container) =>
        container.RegisterSingleton<AnalysisDetailViewFactory>()
                 .RegisterTransient<FileSystemAnalyzer>()
                 .RegisterTransient<IFileSystemAnalysisSession, RavenDbFileSystemAnalysisSession>();
}