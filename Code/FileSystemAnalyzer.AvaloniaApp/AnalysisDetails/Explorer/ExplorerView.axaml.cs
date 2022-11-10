using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Explorer;

public sealed partial class ExplorerView : UserControl
{
    public ExplorerView()
    {
        InitializeComponent();
        ListBoxPager<ExplorerViewModel>.EnableEndlessScrolling(this, ListBox);
    }

    private void OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        if (sender is StyledElement { DataContext: FileSystemEntryViewModel fileSystemEntryViewModel } &&
            DataContext is ExplorerViewModel explorerViewModel)
        {
            e.Handled = true;
            explorerViewModel.SelectFolder(fileSystemEntryViewModel.FileSystemEntry);
        }
    }
}