using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.GuardClauses;
using Synnotech.FluentProcesses;

namespace FileSystemAnalyzer.AvaloniaApp.AppShell;

public sealed class OpenLogFilesFolderCommand : BaseCommand
{
    public OpenLogFilesFolderCommand(OperatingSystem operatingSystem, string logDirectoryPath)
    {
        OperatingSystem = operatingSystem;
        LogDirectoryPath = logDirectoryPath.MustNotBeNullOrWhiteSpace();
    }

    private OperatingSystem OperatingSystem { get; }
    private string LogDirectoryPath { get; }

    public override void Execute()
    {
        // TODO: open finder on Mac OSX, file manager on linux
        if (!OperatingSystem.IsWindows())
            return;
        
        new ProcessBuilder().DisableShellExecute()
                            .DisableExitCodeVerification()
                            .WithFileName("explorer")
                            .WithArguments(LogDirectoryPath)
                            .CreateProcess()
                            .Start();
    }
}