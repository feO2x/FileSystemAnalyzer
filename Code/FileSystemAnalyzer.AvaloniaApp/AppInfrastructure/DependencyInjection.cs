using FileSystemAnalyzer.AvaloniaApp.AnalysesList;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;
using FileSystemAnalyzer.AvaloniaApp.AppShell;
using FileSystemAnalyzer.AvaloniaApp.DataAccess;
using FileSystemAnalyzer.AvaloniaApp.Navigation;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using FileSystemAnalyzer.AvaloniaApp.SystemDialogs;
using LightInject;
using Synnotech.Time;

namespace FileSystemAnalyzer.AvaloniaApp.AppInfrastructure;

public static class DependencyInjection
{
    public static ServiceContainer CreateContainer() =>
        new (new ContainerOptions { EnablePropertyInjection = false });

    public static void ConfigureServices(this ServiceContainer container) =>
        container.RegisterConfiguration(out var configuration)
                 .RegisterLogger(configuration)
                 .RegisterRavenDb(configuration)
                 .RegisterCoreServices()
                 .RegisterSystemDialogs()
                 .RegisterNavigation()
                 .RegisterAnalysisList(configuration)
                 .RegisterAnalysisDetails()
                 .RegisterAppShell(configuration);

    private static IServiceRegistry RegisterCoreServices(this IServiceRegistry container) =>
        container.RegisterInstance(DebouncedValueFactory.DefaultFactory)
                 .RegisterInstance<IClock>(new UtcClock());
    
}