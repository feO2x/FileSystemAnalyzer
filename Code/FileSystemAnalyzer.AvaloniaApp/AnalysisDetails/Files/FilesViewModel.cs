using System;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.ViewModels;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

public sealed class FilesViewModel : BaseNotifyPropertyChanged,
                                     IHasPagingViewModel,
                                     IConverter<object, object>,
                                     ITabItemViewModel
{
    private string _selectedGrouping;

    public FilesViewModel(string analysisId,
                          Func<IFilesSession> createSession,
                          DebouncedValueFactory debouncedValueFactory,
                          ILogger logger)
    {
        var filters = new FilesFilters(analysisId);
        _selectedGrouping = filters.Grouping;
        PagingViewModel = new (createSession, 100, filters, this, logger);
        DebouncedSearchTerm = debouncedValueFactory.CreateDebouncedValue(string.Empty, OnDebouncedSearchTermChanged);
        PagingViewModel.LoadFirstPage();
    }

    public PagingViewModel<object, object, FilesFilters> PagingViewModel { get; }
    private DebouncedValue<string> DebouncedSearchTerm { get; }

    public string SearchTerm
    {
        get => DebouncedSearchTerm.CurrentValue;
        set
        {
            if (DebouncedSearchTerm.TrySetValue(value))
                OnPropertyChanged();
        }
    }

    public string[] AllGroupings => Groupings.All;

    public string SelectedGrouping
    {
        get => _selectedGrouping;
        set
        {
            if (SetIfDifferent(ref _selectedGrouping, value))
                PagingViewModel.Filters = PagingViewModel.Filters with { Grouping = value };
        }
    }

    private bool IsInitialized { get; set; }

    object IConverter<object, object>.Convert(object value) =>
        value is FileSystemEntry fileEntry ?
            new FileViewModel(fileEntry) :
            new GroupByFileExtensionViewModel((GroupByFileExtension) value);

    IPagingViewModel IHasPagingViewModel.PagingViewModel => PagingViewModel;

    public string Title => "Files";

    public void Reload(bool isOptional = true)
    {
        if (isOptional && IsInitialized)
            return;

        PagingViewModel.ReloadAsync(true);
        IsInitialized = true;
    }

    private void OnDebouncedSearchTermChanged() =>
        PagingViewModel.Filters = PagingViewModel.Filters with { SearchTerm = DebouncedSearchTerm.CurrentValue };
}