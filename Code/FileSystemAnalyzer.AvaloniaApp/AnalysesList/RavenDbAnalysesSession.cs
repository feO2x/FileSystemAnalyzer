using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;
using Light.GuardClauses;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Session;
using Synnotech.RavenDB;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

// ReSharper disable once ClassNeverInstantiated.Global -- instantiated by DI container
public sealed class RavenDbAnalysesSession : AsyncReadOnlySession, IAnalysesSession
{
    public RavenDbAnalysesSession(IAsyncDocumentSession session) : base(session) { }

    public async Task RemoveAnalysisAsync(Analysis analysis)
    {
        Session.Delete(analysis.Id);
        await Session.SaveChangesAsync();
        await Session.Advanced
                     .DocumentStore
                     .Operations
                     .SendAsync(new DeleteByQueryOperation($"from FileSystemEntries where AnalysisId = '{analysis.Id}'"));
    }

    public Task<List<Analysis>> GetItemsAsync(SearchTermFilters filters,
                                              int skip,
                                              int take,
                                              CancellationToken cancellationToken)
    {
        IQueryable<Analysis> query = Session.Query<Analysis>();
        var searchTerm = filters.SearchTerm;
        query = !searchTerm.IsNullOrWhiteSpace() ?
            query.Search(analysis => analysis.DirectoryPathForSearch, searchTerm) :
            query.OrderByDescending(analysis => analysis.CreatedAtUtc);

        return query.Skip(skip)
                    .Take(take)
                    .ToListAsync(cancellationToken);
    }
}