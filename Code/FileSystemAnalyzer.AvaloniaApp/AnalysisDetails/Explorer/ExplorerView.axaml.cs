using Avalonia.Controls;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Explorer;

public sealed partial class ExplorerView : UserControl
{
    public ExplorerView()
    {
        InitializeComponent();
        ListBoxPager<ExplorerViewModel>.EnableEndlessScrolling(this, ListBox);
    }
}