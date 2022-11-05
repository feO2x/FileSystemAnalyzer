using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Raven.Client.Documents.Session;
using Synnotech.RavenDB;

namespace FileSystemAnalyzer.AvaloniaApp.FileSystemAnalysis;

public sealed class RavenDbFileSystemAnalysisSession : AsyncSession, IFileSystemAnalysisSession
{
    public RavenDbFileSystemAnalysisSession(IAsyncDocumentSession session) : base(session) { }

    public Task StoreAsync<T>(T entity, CancellationToken cancellationToken) =>
        Session.StoreAsync(entity, cancellationToken);
    
    public Task<FileSystemEntry> GetFileSystemEntryAsync(string id, CancellationToken cancellationToken) =>
        Session.LoadAsync<FileSystemEntry>(id, cancellationToken);
}