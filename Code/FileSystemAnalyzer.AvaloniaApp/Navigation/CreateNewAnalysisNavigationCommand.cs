using FileSystemAnalyzer.AvaloniaApp.AnalysesList;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

namespace FileSystemAnalyzer.AvaloniaApp.Navigation;

// ReSharper disable once ClassNeverInstantiated.Global -- class is instantiated by the DI container
public sealed class CreateNewAnalysisNavigationCommand : ICreateNewAnalysisNavigationCommand
{
    public CreateNewAnalysisNavigationCommand(AnalysisDetailViewModelFactory analysisDetailViewModelFactory, INavigator navigator)
    {
        AnalysisDetailViewModelFactory = analysisDetailViewModelFactory;
        Navigator = navigator;
    }

    private AnalysisDetailViewModelFactory AnalysisDetailViewModelFactory { get; }
    private INavigator Navigator { get; }

    public async void Navigate(string targetDirectoryPath)
    {
        var view = await AnalysisDetailViewModelFactory.CreateForNewAnalysisAsync(targetDirectoryPath);
        Navigator.NavigateTo(view);
    }
}