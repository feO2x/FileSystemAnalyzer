using LightInject;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Explorer;

public static class ExplorerModule
{
    public static IServiceRegistry RegisterExplorerModule(this IServiceRegistry container) =>
        container.RegisterTransient<IExplorerSession, RavenDbExplorerSession>();
}