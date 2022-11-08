using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
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

    public Task<List<FileSystemEntry>> GetFolderContentsAsync(string folderId, CancellationToken cancellationToken) =>
        Session.Query<FileSystemEntry>()
               .Where(entry => entry.ParentId == folderId)
               .OrderBy(entry => entry.Type)
               .ThenBy(entry => entry.Name)
               .ToListAsync(cancellationToken);
}