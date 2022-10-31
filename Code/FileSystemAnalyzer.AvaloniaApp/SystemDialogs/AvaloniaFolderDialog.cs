using System;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace FileSystemAnalyzer.AvaloniaApp.SystemDialogs;

// ReSharper disable once ClassNeverInstantiated.Global -- this class is instantiated by the DI container
public sealed class AvaloniaFolderDialog : IFolderDialog
{
    public AvaloniaFolderDialog(Func<Window> getMainWindow) =>
        GetMainWindow = getMainWindow;

    private Func<Window> GetMainWindow { get; }

    public Task<string?> ChooseFolderAsync()
    {
        var dialog = new OpenFolderDialog();
        var mainWindow = GetMainWindow();
        return dialog.ShowAsync(mainWindow);
    }
}