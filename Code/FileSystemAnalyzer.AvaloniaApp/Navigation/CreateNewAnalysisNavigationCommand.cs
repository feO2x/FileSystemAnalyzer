using FileSystemAnalyzer.AvaloniaApp.AnalysesList;
using FileSystemAnalyzer.AvaloniaApp.FileSystemAnalysis;

namespace FileSystemAnalyzer.AvaloniaApp.Navigation;

public sealed class CreateNewAnalysisNavigationCommand : ICreateNewAnalysisNavigationCommand
{
    public CreateNewAnalysisNavigationCommand(AnalysisDetailViewFactory analysisDetailViewFactory, INavigator navigator)
    {
        AnalysisDetailViewFactory = analysisDetailViewFactory;
        Navigator = navigator;
    }

    private AnalysisDetailViewFactory AnalysisDetailViewFactory { get; }
    private INavigator Navigator { get; }

    public async void Navigate(string targetDirectoryPath)
    {
        var view = await AnalysisDetailViewFactory.CreateForNewAnalysisAsync(targetDirectoryPath);
        Navigator.NavigateTo(view);
    }
}