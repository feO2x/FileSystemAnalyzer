using Avalonia.Controls;
using Bogus;
using FileSystemAnalyzer.AvaloniaApp.DataAccess;
using FileSystemAnalyzer.AvaloniaApp.GettingStarted;
using FileSystemAnalyzer.AvaloniaApp.Navigation;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using FileSystemAnalyzer.AvaloniaApp.SystemDialogs;
using LightInject;

namespace FileSystemAnalyzer.AvaloniaApp.AppInfrastructure;

public static class DependencyInjection
{
    public static ServiceContainer CreateContainer() =>
        new (new ContainerOptions { EnablePropertyInjection = false });

    public static void ConfigureServices(this ServiceContainer container)
    {
        ConfigureBogusRandomizer();
        container.RegisterConfiguration(out var configuration)
                 .RegisterLogger(configuration)
                 .RegisterRavenDb(configuration)
                 .RegisterDebouncedValueFactory()
                 .RegisterSystemDialogs()
                 .RegisterNavigation()
                 .RegisterGettingStarted(configuration)
                 .RegisterAppShell();
    }

    private static void RegisterAppShell(this IServiceRegistry container) =>
        container.RegisterViewAndViewModelAsSingletons<MainWindow, MainWindowViewModel>()
                 .RegisterSingleton<INavigator>(f => f.GetInstance<MainWindowViewModel>())
                 .RegisterSingleton<Window>(f => f.GetInstance<MainWindow>());
    
    private static IServiceRegistry RegisterDebouncedValueFactory(this IServiceRegistry container) =>
        container.RegisterInstance(DebouncedValueFactory.DefaultFactory);
    
    private static void ConfigureBogusRandomizer() => Randomizer.Seed = new (42);
}