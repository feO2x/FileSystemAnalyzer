using System;
using Avalonia;

namespace FileSystemAnalyzer.AvaloniaApp;

public static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        var appBuilder = BuildAvaloniaApp();
        var returnValue = appBuilder.StartWithClassicDesktopLifetime(args);
        if (appBuilder.Instance is IDisposable disposableApp)
            disposableApp.Dispose();
        return returnValue;
    }

    private static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
                  .UsePlatformDetect()
                  .LogToTrace();
}