using Avalonia.Controls;
using FileSystemAnalyzer.AvaloniaApp.AppShell;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

public sealed partial class AnalysesListView : UserControl, IView
{
    public AnalysesListView()
    {
        InitializeComponent();
        ListBoxPager<AnalysesListViewModel>.EnableEndlessScrolling(this, ListBox);
    }

    public string Title => "Analyses";
}