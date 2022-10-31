namespace FileSystemAnalyzer.AvaloniaApp.GettingStarted;

public interface ICreateNewAnalysisNavigationCommand
{
    void Navigate(string targetDirectoryPath);
}