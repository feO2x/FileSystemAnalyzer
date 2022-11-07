using LightInject;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

public static class FilesModule
{
    public static IServiceRegistry RegisterFilesModule(this IServiceRegistry container) =>
        container.RegisterTransient<IFilesSession, RavenDbFilesSession>()
                 .RegisterTransient<FilesViewModel>();
}