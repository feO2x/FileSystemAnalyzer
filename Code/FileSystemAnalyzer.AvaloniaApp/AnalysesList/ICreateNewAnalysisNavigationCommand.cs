namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

public interface ICreateNewAnalysisNavigationCommand
{
    void Navigate(string targetDirectoryPath);
}