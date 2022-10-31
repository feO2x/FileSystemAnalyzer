using System;
using FileSystemAnalyzer.AvaloniaApp.GettingStarted;

namespace FileSystemAnalyzer.AvaloniaApp.Navigation;

public sealed class InitialViewNavigationCommand
{
    public InitialViewNavigationCommand(INavigator navigator, Func<InitialView> resolveInitialView)
    {
        Navigator = navigator;
        ResolveInitialView = resolveInitialView;
    }

    private INavigator Navigator { get; }
    private Func<InitialView> ResolveInitialView { get; }

    public void Navigate() => Navigator.NavigateTo(ResolveInitialView());
}