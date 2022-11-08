using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Light.GuardClauses;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Synnotech.RavenDB;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Folders;

public sealed class RavenDbFoldersSession : AsyncReadOnlySession, IFoldersSession
{
    public RavenDbFoldersSession(IAsyncDocumentSession session) : base(session) { }

    public Task<List<FileSystemEntry>> GetItemsAsync(FolderFilters filters,
                                                     int skip,
                                                     int take,
                                                     CancellationToken cancellationToken)
    {
        var analysisId = filters.AnalysisId;
        var searchTerm = filters.SearchTerm;

        var query = Session.Query<FileSystemEntry>()
                           .Where(entry => entry.AnalysisId == analysisId &&
                                           entry.Type == FileSystemEntryType.Folder);

        if (!searchTerm.IsNullOrWhiteSpace())
            query = query.Search(entry => entry.FullPathForSearch, searchTerm);

        return query.OrderByDescending(entry => entry.SizeInBytes)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync(cancellationToken);
    }
}