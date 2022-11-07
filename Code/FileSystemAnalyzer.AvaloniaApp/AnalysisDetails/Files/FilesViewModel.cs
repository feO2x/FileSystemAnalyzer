using System;
using System.Collections.ObjectModel;
using System.Threading;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.GuardClauses;
using Light.ViewModels;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

public sealed class FilesViewModel : BaseNotifyPropertyChanged
{
    private CancellationTokenSource? _currentTokenSource;
    private bool _hasNoFiles;

    public FilesViewModel(Analysis analysis,
                          Func<IFilesSession> createSession,
                          DebouncedValueFactory debouncedValueFactory,
                          ILogger logger)
    {
        Analysis = analysis;
        CreateSession = createSession;
        Logger = logger;
        DebouncedSearchTerm = debouncedValueFactory.CreateDebouncedValue(string.Empty, OnDebouncedSearchTermChanged);
        LoadFiles();
    }

    private Analysis Analysis { get; }
    private Func<IFilesSession> CreateSession { get; }
    private ILogger Logger { get; }

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

    public ObservableCollection<FileViewModel> Items { get; } = new ();

    public bool IsLoading => CurrentTokenSource is not null;

    private CancellationTokenSource? CurrentTokenSource
    {
        get => _currentTokenSource;
        set
        {
            _currentTokenSource = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    private bool IsAtEnd { get; set; }

    public bool HasNoFiles
    {
        get => _hasNoFiles;
        private set => Set(out _hasNoFiles, value);
    }

    private void OnDebouncedSearchTermChanged()
    {
        IsAtEnd = false;
        Items.Clear();
        CurrentTokenSource?.Cancel();
        LoadFiles();
    }

    public async void LoadFiles()
    {
        if (IsAtEnd || HasNoFiles)
            return;

        const int batchSize = 100;
        var cancellationTokenSource = CurrentTokenSource = new ();
        try
        {
            await using var session = CreateSession();

            var loadedFiles = await session.GetFilesAsync(Analysis.Id, Items.Count, batchSize, SearchTerm, cancellationTokenSource.Token);
            if (loadedFiles.Count < batchSize)
                IsAtEnd = true;
            if (SearchTerm.IsNullOrWhiteSpace() && Items.Count == 0 && loadedFiles.Count == 0)
                HasNoFiles = true;

            Items.AddAsViewModels(loadedFiles);
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