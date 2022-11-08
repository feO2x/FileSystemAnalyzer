using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Synnotech.DatabaseAbstractions;

namespace FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

public interface IPagingSession<TModel, in TFilters> : IAsyncReadOnlySession
{
    Task<List<TModel>> GetItemsAsync(TFilters filters, int skip, int take, CancellationToken cancellationToken);
}