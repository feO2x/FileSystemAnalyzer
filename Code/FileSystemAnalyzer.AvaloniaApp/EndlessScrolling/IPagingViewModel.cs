using System.Threading.Tasks;

namespace FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;

public interface IPagingViewModel
{
    Task LoadNextPageAsync();
}