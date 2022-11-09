using System;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.ViewModels;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Folders;

public sealed class FoldersViewModel : BaseNotifyPropertyChanged,
                                       IHasPagingViewModel,
                                       IConverter<FileSystemEntry, FolderViewModel>,
                                       ITabItemViewModel
{
    public FoldersViewModel(string analysisId,
                            Func<IFoldersSession> createSession,
                            DebouncedValueFactory debouncedValueFactory,
                            ILogger logger)
    {
        PagingViewModel = new (createSession, 100, new (analysisId), this, logger);
        DebouncedSearchTerm = debouncedValueFactory.CreateDebouncedValue(string.Empty, OnDebouncedSearchTermChanged);
        PagingViewModel.LoadFirstPage();
    }

    public PagingViewModel<FolderViewModel, FileSystemEntry, FolderFilters> PagingViewModel { get; }
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
    
    private bool IsInitialized { get; set; }

    FolderViewModel IConverter<FileSystemEntry, FolderViewModel>.Convert(FileSystemEntry value) => new (value);

    IPagingViewModel IHasPagingViewModel.PagingViewModel => PagingViewModel;

    public string Title => "Folders";
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