using FileSystemAnalyzer.AvaloniaApp.AnalysesList;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;

namespace FileSystemAnalyzer.AvaloniaApp.Navigation;

// ReSharper disable once ClassNeverInstantiated.Global -- instantiated by DI container
public sealed class OpenExistingAnalysisNavigationCommand : IOpenExistingAnalysisNavigationCommand
{
    public OpenExistingAnalysisNavigationCommand(AnalysisDetailViewFactory analysisDetailViewFactory,
                                                 INavigator navigator)
    {
        AnalysisDetailViewFactory = analysisDetailViewFactory;
        Navigator = navigator;
    }

    private AnalysisDetailViewFactory AnalysisDetailViewFactory { get; }
    private INavigator Navigator { get; }

    public async void Navigate(Analysis analysis)
    {
        var view = await AnalysisDetailViewFactory.CreateForExistingAnalysisAsync(analysis);
        Navigator.NavigateTo(view);
    }
}