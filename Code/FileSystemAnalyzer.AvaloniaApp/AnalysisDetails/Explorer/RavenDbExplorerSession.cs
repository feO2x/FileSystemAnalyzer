using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Light.GuardClauses;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Synnotech.RavenDB;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Explorer;

public sealed class RavenDbExplorerSession : AsyncReadOnlySession, IExplorerSession
{
    public RavenDbExplorerSession(IAsyncDocumentSession session) : base(session) { }

    public Task<List<FileSystemEntry>> GetAllFoldersAsync(string analysisId) =>
        Session.Query<FileSystemEntry>()
               .Where(entry => entry.Type == FileSystemEntryType.Folder &&
                               entry.AnalysisId == analysisId)
               .ToListAsync();

    public Task<List<FileSystemEntry>> GetItemsAsync(ExplorerFilters filters, int skip, int take, CancellationToken cancellationToken)
    {
        var folderId = filters.ParentFolderId;
        var searchTerm = filters.SearchTerm;
        var query = Session.Query<FileSystemEntry>()
                           .Where(entry => entry.ParentId == folderId);
        if (!searchTerm.IsNullOrWhiteSpace())
            query = query.Search(entry => entry.Name, searchTerm);
        
        return query.OrderBy(entry => entry.Type)
                    .ThenBy(entry => entry.Name)
                    .ToListAsync(cancellationToken);
    }
}