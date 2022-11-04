using System;
using System.Collections.ObjectModel;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.GuardClauses;
using Light.ViewModels;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.GettingStarted;

// ReSharper disable once ClassNeverInstantiated.Global -- the view model is instantiated by the DI container
public sealed class ExistingAnalysesViewModel : BaseNotifyPropertyChanged
{
    private bool _isLoading;
    private bool _hasNoAnalyses;

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

    public bool IsLoading
    {
        get => _isLoading;
        private set => Set(out _isLoading, value);
    }

    private bool IsAtEnd { get; set; }

    public bool HasNoAnalyses
    {
        get => _hasNoAnalyses;
        private set => Set(out _hasNoAnalyses, value);
    }

    public async void LoadAnalyses()
    {
        if (IsAtEnd || IsLoading)
            return;
        
        const int batchSize = 100;
        try
        {
            await using var session = CreateSession();
            IsLoading = true;
            var loadedAnalyses = await session.GetAnalysesAsync(Analyses.Count, batchSize, SearchTerm);
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
            IsLoading = false;
        }
    }
}