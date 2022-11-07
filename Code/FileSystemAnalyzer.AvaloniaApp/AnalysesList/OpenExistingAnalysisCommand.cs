using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.Shared;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

public sealed class OpenExistingAnalysisCommand : BaseCommand<Analysis>
{
    public OpenExistingAnalysisCommand(IOpenExistingAnalysisNavigationCommand navigationCommand) =>
        NavigationCommand = navigationCommand;

    private IOpenExistingAnalysisNavigationCommand NavigationCommand { get; }

    public override void Execute(Analysis analysis) => NavigationCommand.Navigate(analysis);
}