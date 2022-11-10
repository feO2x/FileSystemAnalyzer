using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

public sealed class GroupByFileExtension
{
    public string AnalysisId { get; init; } = string.Empty;

    public FileSystemEntryType Type { get; init; }
    public string FileExtension { get; init; } = string.Empty;
    public int NumberOfFiles { get; init; }
    public long TotalSizeInBytes { get; init; }
}