namespace FileSystemAnalyzer.AvaloniaApp.FileSystemAnalysis;

public sealed record ProgressState(long NumberOfProcessedFolders, long NumberOfProcessedFiles, bool IsFinished = false);