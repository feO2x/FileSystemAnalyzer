using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Humanizer;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

public sealed class FileViewModel
{
    public FileViewModel(FileSystemEntry fileSystemEntry)
    {
        FileSystemEntry = fileSystemEntry;
        Size = fileSystemEntry.SizeInBytes.Bytes().Humanize("#.##");
    }

    public FileSystemEntry FileSystemEntry { get; }

    public string Name => FileSystemEntry.Name;
    public string Size { get; }
    public string FullPath => FileSystemEntry.FullPath;
}

public static class FileViewModelExtensions
{
    public static void AddAsViewModels(this ObservableCollection<FileViewModel> collection, List<FileSystemEntry> entries)
    {
        for (var i = 0; i < entries.Count; i++)
        {
            collection.Add(new (entries[i]));
        }
    }
}