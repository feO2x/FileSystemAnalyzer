using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Synnotech.DatabaseAbstractions;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

public interface IFilesSession : IAsyncReadOnlySession
{
    Task<List<FileSystemEntry>> GetFilesAsync(string analysisId,
                                              int skip,
                                              int take,
                                              string searchTerm,
                                              CancellationToken cancellationToken);
}