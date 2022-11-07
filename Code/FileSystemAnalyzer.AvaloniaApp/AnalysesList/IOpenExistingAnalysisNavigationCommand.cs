using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

public interface IOpenExistingAnalysisNavigationCommand
{
    void Navigate(Analysis analysis);
}