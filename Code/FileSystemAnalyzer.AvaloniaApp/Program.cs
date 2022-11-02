using System;
using Avalonia;
using FileSystemAnalyzer.AvaloniaApp.AppInfrastructure;

namespace FileSystemAnalyzer.AvaloniaApp;

public static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        var appBuilder = BuildAvaloniaApp();
        try
        {
            return appBuilder.StartWithClassicDesktopLifetime(args);
        }
        catch (Exception exception)
        {
            Logging.LogFatalError(exception);
            return 1;
        }
        finally
        {
            if (appBuilder.Instance is IDisposable disposableApp)
                disposableApp.Dispose();
        }
    }

    private static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
                  .UsePlatformDetect()
                  .LogToTrace();
}