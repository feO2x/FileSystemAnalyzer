using System;
using FileSystemAnalyzer.AvaloniaApp.AnalysesList;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

namespace FileSystemAnalyzer.AvaloniaApp.Navigation;

// ReSharper disable once ClassNeverInstantiated.Global -- instantiated by DI container
public sealed class NavigateToAnalysesListCommand : INavigateToAnalysesListCommand
{
    public NavigateToAnalysesListCommand(INavigator navigator, Func<AnalysesListViewModel> resolveInitialView)
    {
        Navigator = navigator;
        ResolveInitialView = resolveInitialView;
    }

    private INavigator Navigator { get; }
    private Func<AnalysesListViewModel> ResolveInitialView { get; }

    public void Navigate() => Navigator.NavigateTo(ResolveInitialView());
}