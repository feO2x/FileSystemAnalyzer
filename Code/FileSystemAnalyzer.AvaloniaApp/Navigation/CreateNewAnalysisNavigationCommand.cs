using FileSystemAnalyzer.AvaloniaApp.GettingStarted;

namespace FileSystemAnalyzer.AvaloniaApp.Navigation;

public sealed class CreateNewAnalysisNavigationCommand : ICreateNewAnalysisNavigationCommand
{
    public CreateNewAnalysisNavigationCommand(INavigator navigator) => Navigator = navigator;

    private INavigator Navigator { get; }

    public void Navigate(string targetDirectoryPath)
    {
        
    }
}