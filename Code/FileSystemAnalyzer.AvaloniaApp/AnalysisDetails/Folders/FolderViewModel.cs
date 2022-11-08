using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Humanizer;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Folders;

public sealed class FolderViewModel
{
    public FolderViewModel(FileSystemEntry folderEntry)
    {
        FolderEntry = folderEntry;
        Size = folderEntry.SizeInBytes.Bytes().Humanize("#.##");
    }

    public FileSystemEntry FolderEntry { get; }

    public string Name => FolderEntry.Name;
    public string Size { get; }
    public string FullPath => FolderEntry.FullPath;
}