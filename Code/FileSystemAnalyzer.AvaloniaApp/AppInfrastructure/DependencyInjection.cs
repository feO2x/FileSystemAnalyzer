using Avalonia.Controls;
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

    public static void ConfigureServices(this ServiceContainer container) =>
        container.RegisterConfiguration(out var configuration)
                 .RegisterRavenDb(configuration)
                 .RegisterSystemDialogs()
                 .RegisterNavigation()
                 .RegisterGettingStarted()
                 .RegisterAppShell();

    private static void RegisterAppShell(this IServiceRegistry container) =>
        container.RegisterViewAndViewModelAsSingletons<MainWindow, MainWindowViewModel>()
                 .RegisterSingleton<INavigator>(f => f.GetInstance<MainWindowViewModel>())
                 .RegisterSingleton<Window>(f => f.GetInstance<MainWindow>());
}