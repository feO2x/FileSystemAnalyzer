using System.Collections.Generic;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Explorer;

public interface IExplorerSession : IPagingSession<FileSystemEntry, ExplorerFilters>
{
    Task<List<FileSystemEntry>> GetAllFoldersAsync(string analysisId);
}