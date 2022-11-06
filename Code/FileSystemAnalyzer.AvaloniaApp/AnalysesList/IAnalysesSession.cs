using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Synnotech.DatabaseAbstractions;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

public interface IAnalysesSession : IAsyncReadOnlySession
{
    Task<List<Analysis>> GetAnalysesAsync(int skip, int take, string searchTerm, CancellationToken cancellationToken);
    Task RemoveAnalysisAsync(Analysis analysis);
}