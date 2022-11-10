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
    private bool _isExpanded;

    public FolderNode(FileSystemEntry folderEntry)
    {
        if (folderEntry.Type != FileSystemEntryType.Folder)
            throw new ArgumentException("Only folders can be represented by a node");

        FolderEntry = folderEntry;
    }

    public FileSystemEntry FolderEntry { get; }
    public string Name => FolderEntry.Name;
    public string Size => FolderEntry.SizeInBytes.ConvertToDisplaySize();

    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetIfDifferent(ref _isExpanded, value);
    }

    public ObservableCollection<FolderNode> ChildNodes { get; } = new ();
    public FolderNode? Parent { get; set; }

    public void AddChildNode(FolderNode childNode)
    {
        childNode.Parent = this;
        ChildNodes.Add(childNode);
    }

    public void RaiseSizeChanged() => OnPropertyChanged(nameof(Size));

    public static FolderNode CreateTreeFromFlatFolderEntries(List<FileSystemEntry> folderEntries, Dictionary<string, FolderNode> nodesLookup)
    {
        var rootNode = CreateNodesFromEntries(folderEntries, nodesLookup);
        ConnectFolderNodes(rootNode, nodesLookup);
        return rootNode;
    }

    private static FolderNode CreateNodesFromEntries(List<FileSystemEntry> folderEntries, Dictionary<string, FolderNode> nodesLookup)
    {
        folderEntries.MustNotBeNullOrEmpty();

        FolderNode? rootNode = null;
        var analysisId = folderEntries[0].AnalysisId;

        for (var i = 0; i < folderEntries.Count; i++)
        {
            var entry = folderEntries[i];
            var folderNode = new FolderNode(entry);
            if (entry.ParentId is null)
                rootNode = folderNode;
            else
                nodesLookup.Add(entry.Id, folderNode);
        }

        if (rootNode is null)
            throw new InvalidStateException($"The loaded folder entries do not contain the root node for analysis \"{analysisId}\"");

        return rootNode;
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

                currentNode.AddChildNode(childNode);
                queue.Enqueue(childNode);
            }
        }
    }
}