using System.Threading.Tasks;

namespace FileSystemAnalyzer.AvaloniaApp.SystemDialogs;

public interface IFolderDialog
{
    Task<string?> ChooseFolderAsync();
}