using System;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.ViewModels;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

public sealed class FilesViewModel : BaseNotifyPropertyChanged,
                                     IHasPagingViewModel,
                                     IConverter<FileSystemEntry, FileViewModel>
{
    public FilesViewModel(string analysisId,
                          Func<IFilesSession> createSession,
                          DebouncedValueFactory debouncedValueFactory,
                          ILogger logger)
    {
        PagingViewModel = new (createSession, 100, new (analysisId), this, logger);
        DebouncedSearchTerm = debouncedValueFactory.CreateDebouncedValue(string.Empty, OnDebouncedSearchTermChanged);
#pragma warning disable CS4014 // we don't care about the load result in the constructor
        PagingViewModel.LoadNextPageAsync();
#pragma warning restore CS4014
    }

    public PagingViewModel<FileViewModel, FileSystemEntry, FilesFilters> PagingViewModel { get; }
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

    FileViewModel IConverter<FileSystemEntry, FileViewModel>.Convert(FileSystemEntry value) => new (value);

    IPagingViewModel IHasPagingViewModel.PagingViewModel => PagingViewModel;

    private void OnDebouncedSearchTermChanged() =>
        PagingViewModel.Filters = PagingViewModel.Filters with { SearchTerm = DebouncedSearchTerm.CurrentValue };
}