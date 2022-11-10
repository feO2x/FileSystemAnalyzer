using FileSystemAnalyzer.AvaloniaApp.AnalysesList;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;

namespace FileSystemAnalyzer.AvaloniaApp.Navigation;

// ReSharper disable once ClassNeverInstantiated.Global -- instantiated by DI container
public sealed class OpenExistingAnalysisNavigationCommand : IOpenExistingAnalysisNavigationCommand
{
    public OpenExistingAnalysisNavigationCommand(AnalysisDetailViewModelFactory analysisDetailViewModelFactory,
                                                 INavigator navigator)
    {
        AnalysisDetailViewModelFactory = analysisDetailViewModelFactory;
        Navigator = navigator;
    }

    private AnalysisDetailViewModelFactory AnalysisDetailViewModelFactory { get; }
    private INavigator Navigator { get; }

    public async void Navigate(Analysis analysis)
    {
        var view = await AnalysisDetailViewModelFactory.CreateForExistingAnalysisAsync(analysis);
        Navigator.NavigateTo(view);
    }
}