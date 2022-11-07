using System;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

public interface IFileSystemAnalyzer
{
    Task AnalyzeFileSystemOnBackgroundThreadAsync(Analysis analysis,
                                                  IProgress<ProgressState> progress,
                                                  CancellationToken cancellationToken = default);
}