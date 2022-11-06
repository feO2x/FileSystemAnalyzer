using System;
using FileSystemAnalyzer.AvaloniaApp.AnalysesList;

namespace FileSystemAnalyzer.AvaloniaApp.Navigation;

public sealed class InitialViewNavigationCommand
{
    public InitialViewNavigationCommand(INavigator navigator, Func<AnalysesListView> resolveInitialView)
    {
        Navigator = navigator;
        ResolveInitialView = resolveInitialView;
    }

    private INavigator Navigator { get; }
    private Func<AnalysesListView> ResolveInitialView { get; }

    public void Navigate() => Navigator.NavigateTo(ResolveInitialView());
}