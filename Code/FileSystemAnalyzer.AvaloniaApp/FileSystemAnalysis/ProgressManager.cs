using System;

namespace FileSystemAnalyzer.AvaloniaApp.FileSystemAnalysis;

public sealed class ProgressManager
{
    public ProgressManager(IProgress<ProgressState> progress) => Progress = progress;

    private IProgress<ProgressState> Progress { get; }
    public long NumberOfProcessedFolders { get; set; }
    public long NumberOfProcessedFiles { get; set; }

    public void ReportNewFile()
    {
        NumberOfProcessedFiles++;
        ReportStateIfNecessary();
    }

    public void ReportNewFolder()
    {
        NumberOfProcessedFolders++;
        ReportStateIfNecessary();
    }

    public void ReportFinish() =>
        Progress.Report(new (NumberOfProcessedFolders, NumberOfProcessedFiles, true));

    private void ReportStateIfNecessary()
    {
        if ((NumberOfProcessedFiles + NumberOfProcessedFolders) % 1000 == 0)
            Progress.Report(new (NumberOfProcessedFolders, NumberOfProcessedFiles));
    }
}