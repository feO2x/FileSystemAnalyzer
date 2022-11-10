using Avalonia.Controls;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

public sealed partial class AnalysesListView : UserControl
{
    public AnalysesListView()
    {
        InitializeComponent();
        ListBoxPager<AnalysesListViewModel>.EnableEndlessScrolling(this, ListBox);
    }
}