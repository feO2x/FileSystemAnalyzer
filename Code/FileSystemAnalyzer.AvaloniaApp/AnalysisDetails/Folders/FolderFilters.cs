using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.GuardClauses;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Folders;

public readonly record struct FolderFilters : IPagingFilters
{
    private readonly string? _searchTerm;

    public FolderFilters(string analysisId)
    {
        AnalysisId = analysisId;
        _searchTerm = string.Empty;
    }

    public string SearchTerm
    {
        get => _searchTerm ?? string.Empty;
        init => _searchTerm = value.NormalizeSearchTerm();
    }
    
    public string AnalysisId { get; }
    
    public bool AreNoFiltersApplied => SearchTerm.IsNullOrWhiteSpace();
}