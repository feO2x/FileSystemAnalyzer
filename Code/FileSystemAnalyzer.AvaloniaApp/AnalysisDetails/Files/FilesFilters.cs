using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.GuardClauses;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

public readonly record struct FilesFilters : IPagingFilters
{
    private readonly string? _searchTerm;
    private readonly string _grouping;

    public FilesFilters(string analysisId)
    {
        AnalysisId = analysisId;
        _searchTerm = string.Empty;
        _grouping = Groupings.NoGrouping;
    }

    public string SearchTerm
    {
        get => _searchTerm ?? string.Empty;
        init => _searchTerm = value.NormalizeSearchTerm();
    }

    public string Grouping
    {
        get => _grouping;
        init => _grouping = value.MustBeOneOf(Groupings.All);
    }

    public string AnalysisId { get; }
    
    public bool AreNoFiltersApplied => SearchTerm.IsNullOrWhiteSpace();
}