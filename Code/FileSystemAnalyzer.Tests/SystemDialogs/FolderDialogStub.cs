using FileSystemAnalyzer.AvaloniaApp.SystemDialogs;

namespace FileSystemAnalyzer.Tests.SystemDialogs;

public sealed class FolderDialogStub : IFolderDialog
{
    public string? ReturnValue { get; set; }
    
    public Task<string?> ChooseFolderAsync() => Task.FromResult(ReturnValue);
}