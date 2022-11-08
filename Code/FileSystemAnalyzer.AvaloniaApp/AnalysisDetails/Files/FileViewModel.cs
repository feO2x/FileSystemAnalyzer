using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.Shared;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

public sealed class FileViewModel
{
    public FileViewModel(FileSystemEntry fileSystemEntry)
    {
        FileSystemEntry = fileSystemEntry;
        Size = fileSystemEntry.SizeInBytes.ConvertToDisplaySize();
    }

    public FileSystemEntry FileSystemEntry { get; }

    public string Name => FileSystemEntry.Name;
    public string Size { get; }
    public string FullPath => FileSystemEntry.FullPath;
}