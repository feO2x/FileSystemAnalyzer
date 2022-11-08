﻿using System;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.ViewModels;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

public sealed class FilesViewModel : BaseNotifyPropertyChanged,
                                     IHasPagingViewModel,
                                     IConverter<FileSystemEntry, FileViewModel>,
                                     ITabItemViewModel
{
    public FilesViewModel(string analysisId,
                          Func<IFilesSession> createSession,
                          DebouncedValueFactory debouncedValueFactory,
                          ILogger logger)
    {
        PagingViewModel = new (createSession, 100, new (analysisId), this, logger);
        DebouncedSearchTerm = debouncedValueFactory.CreateDebouncedValue(string.Empty, OnDebouncedSearchTermChanged);
        PagingViewModel.LoadFirstPage();
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

    public string Title => "Files";
}