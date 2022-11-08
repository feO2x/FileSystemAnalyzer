using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

public interface IAnalysesSession : IPagingSession<Analysis, SearchTermFilters>
{
    Task RemoveAnalysisAsync(Analysis analysis);
}