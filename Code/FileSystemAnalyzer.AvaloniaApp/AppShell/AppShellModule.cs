using Avalonia.Controls;
using FileSystemAnalyzer.AvaloniaApp.Navigation;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using LightInject;

namespace FileSystemAnalyzer.AvaloniaApp.AppShell;

public static class AppShellModule
{
    public static IServiceRegistry RegisterAppShell(this IServiceRegistry container) =>
        container.RegisterViewAndViewModelAsSingletons<MainWindow, MainWindowViewModel>()
                 .RegisterSingleton<INavigator>(f => f.GetInstance<MainWindowViewModel>())
                 .RegisterSingleton<Window>(f => f.GetInstance<MainWindow>())
                 .RegisterInstance<IOperatingSystem>(new OperatingSystem());
}