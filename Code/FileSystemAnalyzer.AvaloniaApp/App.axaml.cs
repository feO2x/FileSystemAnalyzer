using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;
using FileSystemAnalyzer.AvaloniaApp.AppInfrastructure;
using FileSystemAnalyzer.AvaloniaApp.AppShell;
using LightInject;

namespace FileSystemAnalyzer.AvaloniaApp;

// ReSharper disable once ClassNeverInstantiated.Global -- App is instantiated by Avalonia
public sealed class App : Application, IDisposable
{
    public App()
    {
        Container = DependencyInjection.CreateContainer();
        Current = this;
    }

    public new static App? Current { get; private set; }

    public ServiceContainer Container { get; }

    public void Dispose() => Container.Dispose();

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Container.ConfigureServices();
            desktop.MainWindow = Container.GetInstance<MainWindow>();
            Container.GetInstance<INavigateToAnalysesListCommand>().Navigate();
        }

        base.OnFrameworkInitializationCompleted();
    }
}