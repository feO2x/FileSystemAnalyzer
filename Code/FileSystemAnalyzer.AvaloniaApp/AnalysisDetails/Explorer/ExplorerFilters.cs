using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.GuardClauses;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Explorer;

public readonly record struct ExplorerFilters : IPagingFilters
{
    private readonly string _parentFolderId;
    private readonly string _searchTerm;

    public ExplorerFilters()
    {
        _searchTerm = string.Empty;
        _parentFolderId = string.Empty;
    }

    public string SearchTerm
    {
        get => _searchTerm;
        init => _searchTerm = value.NormalizeSearchTerm();
    }

    public string ParentFolderId
    {
        get => _parentFolderId;
        init => _parentFolderId = value.MustNotBeNullOrWhiteSpace();
    }

    public bool AreNoFiltersApplied => false;
}