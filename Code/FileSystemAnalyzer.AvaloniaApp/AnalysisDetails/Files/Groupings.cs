namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

public static class Groupings
{
    public const string NoGrouping = "No Grouping";
    public const string ByFileExtension = "By File Extension";

    public static string[] All { get; } = { NoGrouping, ByFileExtension };
}