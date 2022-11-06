using System;
using System.Collections.ObjectModel;
using System.Threading;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.GuardClauses;
using Light.ViewModels;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

// ReSharper disable once ClassNeverInstantiated.Global -- the view model is instantiated by the DI container
public sealed class ExistingAnalysesViewModel : BaseNotifyPropertyChanged
{
    private bool _hasNoAnalyses;
    private CancellationTokenSource? _currentTokenSource;

    public ExistingAnalysesViewModel(Func<IAnalysesSession> createSession,
                                     DebouncedValueFactory debouncedValueFactory,
                                     ILogger logger)
    {
        CreateSession = createSession;
        DebouncedSearchTerm = debouncedValueFactory.CreateDebouncedValue(string.Empty, OnDebouncedSearchTermChanged);
        Logger = logger;
        
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
}