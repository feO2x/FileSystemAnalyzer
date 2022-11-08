using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.GuardClauses;

namespace FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

public readonly record struct SearchTermFilters : IPagingFilters
{
    private readonly string? _searchTerm;

    public SearchTermFilters(string searchTerm) => _searchTerm = searchTerm.NormalizeSearchTerm();

    public string SearchTerm => _searchTerm ?? string.Empty;

    public bool AreNoFiltersApplied => SearchTerm.IsNullOrWhiteSpace();

    public static SearchTermFilters Create() => new (string.Empty);
}