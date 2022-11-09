﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Explorer;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Folders;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Light.ViewModels;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

public sealed class AnalysisDetailViewModel : BaseNotifyPropertyChanged
{
    private CancellationTokenSource? _cancellationTokenSource;
    private string? _currentProgressState;
    private ITabItemViewModel _selectedTabItemViewModel;

    public AnalysisDetailViewModel(Analysis analysis,
                                   FilesViewModel filesViewModel,
                                   FoldersViewModel foldersViewModel,
                                   ExplorerViewModel explorerViewModel,
                                   IFileSystemAnalyzer? analyzer,
                                   INavigateToAnalysesListCommand navigateCommand,
                                   ILogger logger)
    {
        Analysis = analysis;
        ExplorerViewModel = explorerViewModel;
        TabItemViewModels = new ITabItemViewModel[] { explorerViewModel, filesViewModel, foldersViewModel };
        _selectedTabItemViewModel = filesViewModel;
        Analyzer = analyzer;
        NavigateCommand = navigateCommand;
        Logger = logger;
        CancelCommand = new (CancelAnalysis, () => CancellationTokenSource is not null);
    }

    public Analysis Analysis { get; }
    private ExplorerViewModel ExplorerViewModel { get; }
    public ITabItemViewModel[] TabItemViewModels { get; }

    public ITabItemViewModel SelectedTabItemViewModel
    {
        get => _selectedTabItemViewModel;
        set
        {
            if (SetIfDifferent(ref _selectedTabItemViewModel, value) && Analyzer is null)
                value.Reload();
        }
    }

    private IFileSystemAnalyzer? Analyzer { get; }
    private INavigateToAnalysesListCommand NavigateCommand { get; }
    private ILogger Logger { get; }

    private CancellationTokenSource? CancellationTokenSource
    {
        get => _cancellationTokenSource;
        set
        {
            _cancellationTokenSource = value;
            CancelCommand.RaiseCanExecuteChanged();
        }
    }
    
    public string? CurrentProgressState
    {
        get => _currentProgressState;
        private set => Set(out _currentProgressState, value);
    }

    public DelegateCommand CancelCommand { get; }

    public async void StartAnalysis()
    {
        if (Analyzer is null)
            return;

        var cancellationTokenSource = CancellationTokenSource = new ();
        try
        {
            var progress = new Progress<ProgressState>(HandleProgressUpdate);
            var task = Analyzer.AnalyzeFileSystemOnBackgroundThreadAsync(Analysis, progress, cancellationTokenSource.Token);
            PeriodicallyReload();
            await task;

            foreach (var tabItemViewModel in TabItemViewModels)
            {
                tabItemViewModel.Reload(isOptional: false);
            }
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Could not analyse tree");
        }
        finally
        {
            cancellationTokenSource.Dispose();
            if (ReferenceEquals(cancellationTokenSource, CancellationTokenSource))
                CancellationTokenSource = null;
        }
    }

    private async void PeriodicallyReload()
    {
        while (CancellationTokenSource is not null)
        {
            await Task.Delay(TimeSpan.FromSeconds(6));
            SelectedTabItemViewModel.Reload();
        }
    }

    private void CancelAnalysis() => CancellationTokenSource?.Cancel();

    private void HandleProgressUpdate(ProgressState progressState)
    {
        CurrentProgressState = progressState.IsFinished ?
            null :
            $"{progressState.NumberOfProcessedFolders} folders / {progressState.NumberOfProcessedFiles} files processed...";

        if (progressState.newFolder is not null)
            ExplorerViewModel.InsertFolder(progressState.newFolder);
    }

    public void NavigateBack() => NavigateCommand.Navigate();
}