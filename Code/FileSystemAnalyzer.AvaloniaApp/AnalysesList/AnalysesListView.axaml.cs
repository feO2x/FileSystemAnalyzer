using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Light.GuardClauses;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

public sealed partial class AnalysesListView : UserControl
{
    public AnalysesListView()
    {
        InitializeComponent();
        Subscription = ListBox.GetObservable<IScrollable>(ListBox.ScrollProperty)
                              .Subscribe(OnListBoxScrollChanged);
    }
    
    private IDisposable? Subscription { get; set; }
    
    private void OnListBoxScrollChanged(IScrollable scrollable)
    {
        if (scrollable is not ScrollViewer scrollViewer)
            return;

        scrollViewer.ScrollChanged += OnScrollChanged;
        Subscription!.Dispose();
        Subscription = null;
    }
    
    private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (e.OffsetDelta.Y.IsApproximately(0))
            return;

        if (DataContext is not AnalysesListViewModel viewModel || sender is not ScrollViewer scrollViewer)
            return;

        var totalHeight = scrollViewer.Offset.Y + scrollViewer.Viewport.Height;
        if (totalHeight > 0.9 * scrollViewer.Extent.Height)
            viewModel.LoadAnalyses();
    }
}