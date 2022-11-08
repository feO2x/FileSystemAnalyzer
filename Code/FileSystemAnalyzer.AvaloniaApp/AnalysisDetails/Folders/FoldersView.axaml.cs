using Avalonia.Controls;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Folders;

public sealed partial class FoldersView : UserControl
{
    public FoldersView()
    {
        InitializeComponent();
        ListBoxPager<FoldersViewModel>.EnableEndlessScrolling(this, ListBox);
    }
}