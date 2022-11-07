using System;
using System.IO;
using Avalonia.Controls;
using FileSystemAnalyzer.AvaloniaApp.Navigation;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using LightInject;
using Microsoft.Extensions.Configuration;

namespace FileSystemAnalyzer.AvaloniaApp.AppShell;

public static class AppShellModule
{
    public static IServiceRegistry RegisterAppShell(this IServiceRegistry container, IConfiguration configuration)
    {
        var operatingSystem = new OperatingSystem();
        var logFolder = Path.GetDirectoryName(configuration["logging:logFilePath"])!;
        logFolder = Environment.ExpandEnvironmentVariables(logFolder);
        return container.RegisterViewAndViewModelAsSingletons<MainWindow, MainWindowViewModel>()
                        .RegisterSingleton<INavigator>(f => f.GetInstance<MainWindowViewModel>())
                        .RegisterSingleton<Window>(f => f.GetInstance<MainWindow>())
                        .RegisterInstance<IOperatingSystem>(operatingSystem)
                        .RegisterInstance(new OpenLogFilesFolderCommand(operatingSystem, logFolder));
    }
}