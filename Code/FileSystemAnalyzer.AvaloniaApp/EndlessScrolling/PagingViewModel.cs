using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.GuardClauses;
using Light.ViewModels;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

public sealed class PagingViewModel<TItem, TModel, TFilters> : BaseNotifyPropertyChanged, IPagingViewModel
    where TFilters : IPagingFilters
{
    private CancellationTokenSource? _currentCancellationTokenSource;
    private TFilters _filters;
    private bool _hasNoItems;
    private bool _isAtEnd;

    public PagingViewModel(Func<IPagingSession<TModel, TFilters>> createSession,
                           int pageSize,
                           TFilters initialFilters,
                           IConverter<TModel, TItem> modelToItemConverter,
                           ILogger logger)
    {
        CreateSession = createSession;
        ModelToItemConverter = modelToItemConverter;
        _filters = initialFilters;
        Logger = logger;
        PageSize = pageSize.MustBeGreaterThan(0);
    }

    private Func<IPagingSession<TModel, TFilters>> CreateSession { get; }
    private IConverter<TModel, TItem> ModelToItemConverter { get; }
    private ILogger Logger { get; }
    public int PageSize { get; }
    public ObservableCollection<TItem> Items { get; } = new ();

    public TFilters Filters
    {
        get => _filters;
        set
        {
            _filters = value;
            ReloadAsync();
        }
    }

    public bool IsAtEnd
    {
        get => _isAtEnd;
        private set => SetIfDifferent(ref _isAtEnd, value);
    }

    public bool HasNoItems
    {
        get => _hasNoItems;
        private set => SetIfDifferent(ref _hasNoItems, value);
    }

    private CancellationTokenSource? CurrentCancellationTokenSource
    {
        get => _currentCancellationTokenSource;
        set
        {
            _currentCancellationTokenSource = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    public bool IsLoading => CurrentCancellationTokenSource is not null;

#pragma warning disable CS4014 // This method is usually called within constructors which cannot be async and do not care about the result 
    public void LoadFirstPage() => LoadNextPageAsync();
#pragma warning restore CS4014

    public async Task LoadNextPageAsync()
    {
        if (IsAtEnd || HasNoItems)
            return;

        var cancellationTokenSource = CurrentCancellationTokenSource = new ();
        var filters = Filters;
        try
        {
            await using var session = CreateSession();

            var loadedItems = await session.GetItemsAsync(filters, Items.Count, PageSize, cancellationTokenSource.Token);
            if (loadedItems.Count < PageSize)
                IsAtEnd = true;
            if (filters.AreNoFiltersApplied && Items.Count == 0 && loadedItems.Count == 0)
                HasNoItems = true;

            for (var i = 0; i < loadedItems.Count; i++)
            {
                Items.Add(ModelToItemConverter.Convert(loadedItems[i]));
            }
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Could not load items");
        }
        finally
        {
            cancellationTokenSource.Dispose();
            if (ReferenceEquals(CurrentCancellationTokenSource, cancellationTokenSource))
                CurrentCancellationTokenSource = null;
        }
    }

    public Task ReloadAsync(bool resetHasNoItems = false)
    {
        IsAtEnd = false;
        Items.Clear();
        CurrentCancellationTokenSource?.Cancel();
        if (resetHasNoItems)
            HasNoItems = false;

        return LoadNextPageAsync();
    }
}