using System;
using System.Collections.ObjectModel;
using System.Threading;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.GuardClauses;
using Light.ViewModels;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

public sealed class AnalysesListViewModel : BaseNotifyPropertyChanged
{
    private bool _hasNoAnalyses;
    private CancellationTokenSource? _currentTokenSource;
    private AnalysisViewModel? _selectedAnalysis;

    public AnalysesListViewModel(Func<IAnalysesSession> createSession,
                                 DebouncedValueFactory debouncedValueFactory,
                                 ILogger logger)
    {
        CreateSession = createSession;
        DebouncedSearchTerm = debouncedValueFactory.CreateDebouncedValue(string.Empty, OnDebouncedSearchTermChanged);
        Logger = logger;

        DeleteSelectedAnalysisCommand = new (DeleteSelectedAnalysis, () => SelectedAnalysis is not null);
        
        LoadAnalyses();
    }
    
    private void OnDebouncedSearchTermChanged()
    {
        IsAtEnd = false;
        Analyses.Clear();
        CurrentTokenSource?.Cancel();
        LoadAnalyses();
    }

    private Func<IAnalysesSession> CreateSession { get; }
    private DebouncedValue<string> DebouncedSearchTerm { get; }
    private ILogger Logger { get; }
    public ObservableCollection<AnalysisViewModel> Analyses { get; } = new ();

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

    public bool IsLoading => CurrentTokenSource is not null;
    
    private bool IsAtEnd { get; set; }

    public bool HasNoAnalyses
    {
        get => _hasNoAnalyses;
        private set => Set(out _hasNoAnalyses, value);
    }
    
    public DelegateCommand DeleteSelectedAnalysisCommand { get; }

    private CancellationTokenSource? CurrentTokenSource
    {
        get => _currentTokenSource;
        set
        {
            _currentTokenSource = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    public async void LoadAnalyses()
    {
        if (IsAtEnd || HasNoAnalyses)
            return;
        
        const int batchSize = 100;
        var cancellationTokenSource = CurrentTokenSource = new ();
        try
        {
            await using var session = CreateSession();
            
            var loadedAnalyses = await session.GetAnalysesAsync(Analyses.Count, batchSize, SearchTerm, cancellationTokenSource.Token);
            if (loadedAnalyses.Count < batchSize)
                IsAtEnd = true;
            if (SearchTerm.IsNullOrWhiteSpace() && Analyses.Count == 0 && loadedAnalyses.Count == 0)
                HasNoAnalyses = true;

            Analyses.AppendAsViewModels(loadedAnalyses);
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Could not load existing analyses");
        }
        finally
        {
            cancellationTokenSource.Dispose();
            if (ReferenceEquals(CurrentTokenSource, cancellationTokenSource))
                CurrentTokenSource = null;
        }
    }

    private async void DeleteSelectedAnalysis()
    {
        if (SelectedAnalysis is null)
            return;

        await using var session = CreateSession();
        await session.RemoveAnalysisAsync(SelectedAnalysis.Analysis);
        Analyses.Remove(SelectedAnalysis);
        SelectedAnalysis = null;
    }
}