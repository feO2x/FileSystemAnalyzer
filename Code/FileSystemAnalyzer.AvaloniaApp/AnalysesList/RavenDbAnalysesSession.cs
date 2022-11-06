using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Light.GuardClauses;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Synnotech.RavenDB;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

// ReSharper disable once ClassNeverInstantiated.Global -- instantiated by DI container
public sealed class RavenDbAnalysesSession : AsyncReadOnlySession, IAnalysesSession
{
    public RavenDbAnalysesSession(IAsyncDocumentSession session) : base(session) { }
    
    public Task<List<Analysis>> GetAnalysesAsync(int skip,
                                                 int take,
                                                 string? searchTerm,
                                                 CancellationToken cancellationToken)
    {
        IQueryable<Analysis> query = Session.Query<Analysis>();
        query = !searchTerm.IsNullOrWhiteSpace() ?
            query.Search(analysis => analysis.DirectoryPathForSearch, searchTerm) :
            query.OrderByDescending(analysis => analysis.CreatedAtUtc);  

        return query.Skip(skip)
                    .Take(take)
                    .ToListAsync(cancellationToken);
    }
}