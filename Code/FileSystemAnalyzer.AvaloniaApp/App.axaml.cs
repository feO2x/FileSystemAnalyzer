using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FileSystemAnalyzer.AvaloniaApp.AppInfrastructure;
using FileSystemAnalyzer.AvaloniaApp.Navigation;
using LightInject;

namespace FileSystemAnalyzer.AvaloniaApp;

// ReSharper disable once ClassNeverInstantiated.Global -- App is instantiated by Avalonia
public sealed class App : Application
{
    public App() => Container = DependencyInjection.CreateContainer();

    private ServiceContainer Container { get; }
    
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Container.ConfigureServices();
            desktop.MainWindow = Container.GetInstance<MainWindow>();
            Container.GetInstance<InitialViewNavigationCommand>().Navigate();
        }

        base.OnFrameworkInitializationCompleted();
    }
}