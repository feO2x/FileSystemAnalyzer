using System.Collections.Generic;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Synnotech.DatabaseAbstractions;

namespace FileSystemAnalyzer.AvaloniaApp.GettingStarted;

public interface IAnalysesSession : IAsyncReadOnlySession
{
    Task<List<Analysis>> GetAnalysesAsync(int skip, int take, string searchTerm);
}