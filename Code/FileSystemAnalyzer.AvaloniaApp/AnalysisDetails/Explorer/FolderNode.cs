using System;
using System.Collections.ObjectModel;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.ViewModels;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Explorer;

public sealed class FolderNode : BaseNotifyPropertyChanged
{
    public FolderNode(FileSystemEntry folderEntry)
    {
        if (folderEntry.Type != FileSystemEntryType.Folder)
            throw new ArgumentException("Only folders can be represented by a node");
        
        FolderEntry = folderEntry;
    }
    
    public FileSystemEntry FolderEntry { get; }
    public string Name => FolderEntry.Name;
    public string Size => FolderEntry.SizeInBytes.ConvertToDisplaySize();

    public ObservableCollection<FolderNode> ChildNodes { get; } = new ();

    public void RaiseSizeUpdated() => OnPropertyChanged(Size);
}