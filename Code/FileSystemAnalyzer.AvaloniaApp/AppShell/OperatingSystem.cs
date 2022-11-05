namespace FileSystemAnalyzer.AvaloniaApp.AppShell;

public sealed class OperatingSystem : IOperatingSystem
{
    public bool IsWindows() => System.OperatingSystem.IsWindows();
}