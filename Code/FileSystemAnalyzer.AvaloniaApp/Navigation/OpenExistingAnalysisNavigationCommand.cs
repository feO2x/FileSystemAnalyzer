using FileSystemAnalyzer.AvaloniaApp.AnalysesList;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;

namespace FileSystemAnalyzer.AvaloniaApp.Navigation;

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

    public void Navigate(Analysis analysis)
    {
        var view = AnalysisDetailViewFactory.CreateForExistingAnalysis(analysis);
        Navigator.NavigateTo(view);
    }
}