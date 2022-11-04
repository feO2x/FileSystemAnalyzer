using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Light.GuardClauses;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Synnotech.RavenDB;

namespace FileSystemAnalyzer.AvaloniaApp.GettingStarted;

// ReSharper disable once ClassNeverInstantiated.Global -- instantiated by DI container
public sealed class RavenDbAnalysesSession : AsyncReadOnlySession, IAnalysesSession
{
    public RavenDbAnalysesSession(IAsyncDocumentSession session) : base(session) { }
    
    public Task<List<Analysis>> GetAnalysesAsync(int skip, int take, string? searchTerm)
    {
        var query = Session.Query<Analysis>();
        if (!searchTerm.IsNullOrWhiteSpace())
            query.Search(analysis => analysis.DirectoryPathForSearch, searchTerm);

        return query.OrderByDescending(analysis => analysis.CreatedAtUtc)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();
    }
}