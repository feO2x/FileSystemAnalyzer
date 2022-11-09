using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Material.Icons;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Explorer;

public sealed class FileSystemEntryViewModel
{
    public FileSystemEntryViewModel(FileSystemEntry fileSystemEntry)
    {
        FileSystemEntry = fileSystemEntry;
        Size = fileSystemEntry.SizeInBytes.ConvertToDisplaySize();
        IconKind = fileSystemEntry.Type == FileSystemEntryType.Folder ? MaterialIconKind.Folder : MaterialIconKind.File;
    }

    public FileSystemEntry FileSystemEntry { get; }
    public string Name => FileSystemEntry.Name;
    public string Size { get; }
    public MaterialIconKind IconKind { get; }
}