using System;
using FileSystemAnalyzer.AvaloniaApp.AppShell;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.ViewModels;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

public sealed class AnalysesListViewModel : BaseNotifyPropertyChanged,
                                            IHasPagingViewModel,
                                            IConverter<Analysis, AnalysisViewModel>,
                                            IView
{
    private AnalysisViewModel? _selectedAnalysis;

    public AnalysesListViewModel(Func<IAnalysesSession> createSession,
                                 DebouncedValueFactory debouncedValueFactory,
                                 ILogger logger)
    {
        CreateSession = createSession;
        DebouncedSearchTerm = debouncedValueFactory.CreateDebouncedValue(string.Empty, OnDebouncedSearchTermChanged);
        PagingViewModel = new (createSession, 100, SearchTermFilters.Create(), this, logger);
        DeleteSelectedAnalysisCommand = new (DeleteSelectedAnalysis, () => SelectedAnalysis is not null);

        PagingViewModel.LoadFirstPage();
    }

    public PagingViewModel<AnalysisViewModel, Analysis, SearchTermFilters> PagingViewModel { get; }

    private Func<IAnalysesSession> CreateSession { get; }
    private DebouncedValue<string> DebouncedSearchTerm { get; }

    public AnalysisViewModel? SelectedAnalysis
    {
        get => _selectedAnalysis;
        set
        {
            if (SetIfDifferent(ref _selectedAnalysis, value))
                DeleteSelectedAnalysisCommand.RaiseCanExecuteChanged();
        }
    }

    public string SearchTerm
    {
        get => DebouncedSearchTerm.CurrentValue;
        set
        {
            if (DebouncedSearchTerm.TrySetValue(value))
                OnPropertyChanged();
        }
    }

    public DelegateCommand DeleteSelectedAnalysisCommand { get; }

    AnalysisViewModel IConverter<Analysis, AnalysisViewModel>.Convert(Analysis model) => new (model);

    IPagingViewModel IHasPagingViewModel.PagingViewModel => PagingViewModel;

    public object Title => "Analyses";

    private void OnDebouncedSearchTermChanged() =>
        PagingViewModel.Filters = new (DebouncedSearchTerm.CurrentValue);

    private async void DeleteSelectedAnalysis()
    {
        if (SelectedAnalysis is null)
            return;

        await using var session = CreateSession();
        await session.RemoveAnalysisAsync(SelectedAnalysis.Analysis);
        PagingViewModel.Items.Remove(SelectedAnalysis);
        SelectedAnalysis = null;
    }
}