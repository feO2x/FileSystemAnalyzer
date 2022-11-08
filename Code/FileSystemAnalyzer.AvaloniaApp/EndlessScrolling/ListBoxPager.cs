using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Light.GuardClauses;
using Range = Light.GuardClauses.Range;

namespace FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

public sealed class ListBoxPager<TViewModel>
    where TViewModel : IHasPagingViewModel
{
    private ListBoxPager(StyledElement view, ListBox listBox, double thresholdPercentage)
    {
        View = view;
        ListBox = listBox;
        ThresholdPercentage = thresholdPercentage.MustBeIn(Range.FromExclusive(0.0).ToInclusive(1.0));
    }

    private StyledElement View { get; }
    private double ThresholdPercentage { get; }
    private ListBox ListBox { get; }
    private IDisposable? Subscription { get; set; }

    private void ObtainReferenceToScrollViewerViaObservable()
    {
        Subscription = ListBox.GetObservable<IScrollable>(ListBox.ScrollProperty)
                              .Subscribe(OnListBoxScrollChanged);
    }

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
        if (View.DataContext is not TViewModel viewModel || sender is not ScrollViewer scrollViewer)
            return;

        if (Paging.CheckIfScrollIsNearTheEnd(scrollViewer.Offset.Y,
                                             e.OffsetDelta.Y,
                                             scrollViewer.Viewport.Height,
                                             scrollViewer.Extent.Height,
                                             ThresholdPercentage))
        {
            viewModel.PagingViewModel.LoadNextPageAsync();
        }
    }

    public static void EnableEndlessScrolling(StyledElement view, ListBox listBox, double thresholdPercentage = 0.9) =>
        new ListBoxPager<TViewModel>(view, listBox, thresholdPercentage).ObtainReferenceToScrollViewerViaObservable();
}