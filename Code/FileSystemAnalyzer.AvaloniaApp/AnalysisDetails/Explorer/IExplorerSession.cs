using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Synnotech.DatabaseAbstractions;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Explorer;

public interface IExplorerSession : IAsyncReadOnlySession
{
    Task<List<FileSystemEntry>> GetAllFoldersAsync(string analysisId);
    Task<List<FileSystemEntry>> GetFolderContentsAsync(string folderId, CancellationToken cancellationToken);
}