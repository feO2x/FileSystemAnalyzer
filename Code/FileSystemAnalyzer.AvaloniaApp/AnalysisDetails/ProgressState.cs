namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

public sealed record ProgressState(long NumberOfProcessedFolders, long NumberOfProcessedFiles, bool IsFinished = false);