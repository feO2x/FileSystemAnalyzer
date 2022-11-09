using System;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

public sealed class ProgressManager
{
    public ProgressManager(IProgress<ProgressState> progress) => Progress = progress;

    private IProgress<ProgressState> Progress { get; }
    private int UpdatesSinceLastReport { get; set; }
    public long NumberOfProcessedFolders { get; set; }
    public long NumberOfProcessedFiles { get; set; }

    public void ReportNewFile()
    {
        NumberOfProcessedFiles++;
        UpdatesSinceLastReport++;
        if (UpdatesSinceLastReport > 1_000)
            SendState(new (NumberOfProcessedFolders, NumberOfProcessedFiles));

    }

    public void ReportNewFolder(FileSystemEntry newFolder)
    {
        NumberOfProcessedFolders++;
        SendState(new (NumberOfProcessedFolders, NumberOfProcessedFiles, newFolder));
    }

    public void ReportFinish() =>
        SendState(new (NumberOfProcessedFolders, NumberOfProcessedFiles, IsFinished: true));

    private void SendState(ProgressState state)
    {
        Progress.Report(state);
        UpdatesSinceLastReport = 0;
    }
}