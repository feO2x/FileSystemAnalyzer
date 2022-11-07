using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Synnotech.DatabaseAbstractions;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

public interface IFileSystemAnalysisSession : IAsyncSession
{
    Task StoreAsync<T>(T entity, CancellationToken cancellationToken);
    Task<FileSystemEntry> GetFileSystemEntryAsync(string id, CancellationToken cancellationToken);
}