namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

public interface ITabItemViewModel
{
    string Title { get; }
    void Reload(bool isOptional = true);
}