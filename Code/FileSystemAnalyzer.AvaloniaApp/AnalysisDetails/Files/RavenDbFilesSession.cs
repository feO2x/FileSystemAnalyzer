using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Synnotech.RavenDB;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

// ReSharper disable once ClassNeverInstantiated.Global -- instantiated by DI container
public sealed class RavenDbFilesSession : AsyncReadOnlySession, IFilesSession
{
    public RavenDbFilesSession(IAsyncDocumentSession session) : base(session) { }

    public Task<List<object>> GetItemsAsync(FilesFilters filters, int skip, int take, CancellationToken cancellationToken) =>
        filters.Grouping == Groupings.ByFileExtension ?
            GetItemsByFileExtensionAsync(filters, skip, take, cancellationToken) :
            GetItemsWithNoGroupingAsync(filters, skip, take, cancellationToken);

    private Task<List<object>> GetItemsWithNoGroupingAsync(FilesFilters filters, int skip, int take, CancellationToken cancellationToken)
    {
        var analysisId = filters.AnalysisId;
        var query = Session.Query<FileSystemEntry>()
                           .Where(entry => entry.AnalysisId == analysisId &&
                                           entry.Type == FileSystemEntryType.File);

        var searchTerm = filters.SearchTerm;
        if (searchTerm != string.Empty)
            query = query.Search(entry => entry.FullPathForSearch, searchTerm);

        return query.OrderByDescending(entry => entry.SizeInBytes)
                    .Skip(skip)
                    .Take(take)
                    .Select(entry => (object) entry)
                    .ToListAsync(cancellationToken);
    }

    private async Task<List<object>> GetItemsByFileExtensionAsync(FilesFilters filters, int skip, int take, CancellationToken cancellationToken)
    {
        var analysisId = filters.AnalysisId;

        var query = Session.Query<FileSystemEntry>()
                           .GroupBy(entry => new
                            {
                                entry.AnalysisId,
                                entry.Type,
                                entry.FileExtension
                            })
                           .Select(group => new GroupByFileExtension
                            {
                                AnalysisId = group.Key.AnalysisId,
                                Type = group.Key.Type,
                                FileExtension = group.Key.FileExtension!,
                                NumberOfFiles = group.Count(),
                                TotalSizeInBytes = group.Sum(entry => entry.SizeInBytes)
                            })
                           .Where(group => group.AnalysisId == analysisId &&
                                           group.Type == FileSystemEntryType.File);

        var searchTerm = filters.SearchTerm;
        if (searchTerm != string.Empty)
            query = query.Search(group => group.FileExtension, searchTerm);

        return (await query.OrderByDescending(group => group.TotalSizeInBytes)
                           .Skip(skip)
                           .Take(take)
                           .ToListAsync(cancellationToken))
              .Cast<object>()
              .ToList();
    }
}