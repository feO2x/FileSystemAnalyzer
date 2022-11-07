using System;
using System.Threading;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Light.ViewModels;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

public sealed class AnalysisDetailViewModel : BaseNotifyPropertyChanged
{
    private CancellationTokenSource? _cancellationTokenSource;
    private string? _currentProgressState;

    public AnalysisDetailViewModel(Analysis analysis,
                                   IFileSystemAnalyzer? analyzer,
                                   INavigateToAnalysesListCommand navigateCommand,
                                   ILogger logger)
    {
        Analysis = analysis;
        Analyzer = analyzer;
        NavigateCommand = navigateCommand;
        Logger = logger;
        CancelCommand = new (CancelAnalysis, () => CancellationTokenSource is not null);
        PerformAnalysis();
    }

    public Analysis Analysis { get; }
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

    private async void PerformAnalysis()
    {
        if (Analyzer is null)
            return;

        var cancellationTokenSource = CancellationTokenSource = new ();
        try
        {
            var progress = new Progress<ProgressState>(HandleProgressUpdate);
            await Analyzer.AnalyzeFileSystemOnBackgroundThreadAsync(Analysis, progress, cancellationTokenSource.Token);
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Could not analyse tree");
        }
        finally
        {
            cancellationTokenSource.Dispose();
            CancellationTokenSource = null;
        }
    }

    private void CancelAnalysis() => CancellationTokenSource?.Cancel();

    private void HandleProgressUpdate(ProgressState progressState)
    {
        CurrentProgressState = progressState.IsFinished ?
            null :
            $"{progressState.NumberOfProcessedFolders} folders / {progressState.NumberOfProcessedFiles} files processed...";
    }

    public void NavigateBack() => NavigateCommand.Navigate();
}