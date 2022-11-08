using Avalonia.Controls;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

public sealed partial class FilesView : UserControl
{
    public FilesView()
    {
        InitializeComponent();
        ListBoxPager<FilesViewModel>.EnableEndlessScrolling(this, ListBox);
    }
}