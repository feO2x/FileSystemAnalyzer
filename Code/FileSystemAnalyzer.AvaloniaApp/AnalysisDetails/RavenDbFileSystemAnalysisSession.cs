using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Raven.Client.Documents.Session;
using Synnotech.RavenDB;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

// ReSharper disable once ClassNeverInstantiated.Global -- instantiated by the DI Container
public sealed class RavenDbFileSystemAnalysisSession : AsyncSession, IFileSystemAnalysisSession
{
    public RavenDbFileSystemAnalysisSession(IAsyncDocumentSession session) : base(session)
    {
        // We want to use a single session instance when analyzing and
        // persisting entries. Depending on the amount of files and sub folders,
        // we might easily hit the standard 30 requests per session maximum.
        // RavenDB will throw an exception when we surpass this amount of requests.
        // In this case, we simply avoid the issue by setting MaxNumberOfRequestPerSession to int.MaxValue.
        session.Advanced.MaxNumberOfRequestsPerSession = int.MaxValue;
    }

    public Task StoreAsync<T>(T entity, CancellationToken cancellationToken) =>
        Session.StoreAsync(entity, cancellationToken);
    
    public Task<FileSystemEntry> GetFileSystemEntryAsync(string id, CancellationToken cancellationToken) =>
        Session.LoadAsync<FileSystemEntry>(id, cancellationToken);

    public void EvictFileSystemEntries(List<FileSystemEntry> entries)
    {
        for (var i = 0; i < entries.Count; i++)
        {
            Session.Advanced.Evict(entries[i]);
        }
    }
}