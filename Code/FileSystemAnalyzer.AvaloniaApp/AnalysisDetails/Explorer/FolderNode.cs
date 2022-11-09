using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;
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

    public static FolderNode CreateTreeFromFlatFolderEntries(List<FileSystemEntry> folderEntries)
    {
        var (rootNode, nodesLookup) = CreateNodesFromEntries(folderEntries);
        ConnectFolderNodes(rootNode, nodesLookup);
        return rootNode;
    }

    private static (FolderNode rootNode, Dictionary<string, FolderNode> nodesLookup) CreateNodesFromEntries(List<FileSystemEntry> folderEntries)
    {
        folderEntries.MustNotBeNullOrEmpty();

        var lookup = new Dictionary<string, FolderNode>();
        FolderNode? rootNode = null;
        var analysisId = folderEntries[0].AnalysisId;

        for (var i = 0; i < folderEntries.Count; i++)
        {
            var entry = folderEntries[i];
            var folderNode = new FolderNode(entry);
            if (entry.ParentId is null)
                rootNode = folderNode;
            else
                lookup.Add(entry.Id, folderNode);
        }

        if (rootNode is null)
            throw new InvalidStateException($"The loaded folder entries do not contain the root node for analysis \"{analysisId}\"");

        return (rootNode, lookup);
    }

    private static void ConnectFolderNodes(FolderNode rootNode, Dictionary<string, FolderNode> nodesLookup)
    {
        var queue = new Queue<FolderNode>();
        queue.Enqueue(rootNode);

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();

            var childIds = currentNode.FolderEntry.ChildIds;
            if (childIds.IsNullOrEmpty())
                continue;

            for (var i = 0; i < childIds.Count; i++)
            {
                var childId = childIds[i];
                if (!nodesLookup.TryGetValue(childId, out var childNode))
                    continue;

                currentNode.ChildNodes.Add(childNode);
                queue.Enqueue(childNode);
            }
        }
    }

    public void RaiseSizeChanged() => OnPropertyChanged(nameof(Size));
}