using LightInject;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Folders;

public static class FoldersModule
{
    public static IServiceRegistry RegisterFoldersModule(this IServiceRegistry container) =>
        container.RegisterTransient<IFoldersSession, RavenDbFoldersSession>();
}