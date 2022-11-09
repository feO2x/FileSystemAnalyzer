using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

public sealed record ProgressState(long NumberOfProcessedFolders,
                                   long NumberOfProcessedFiles,
                                   FileSystemEntry? newFolder = null,
                                   bool IsFinished = false);