using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;
using Light.ViewModels;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Explorer;

public sealed class ExplorerViewModel : BaseNotifyPropertyChanged,
                                        ITabItemViewModel
{
    private FolderNode? _selectedFolderNode;

    public ExplorerViewModel(string analysisId, Func<IExplorerSession> createSession)
    {
        AnalysisId = analysisId;
        CreateSession = createSession;
    }

    private string AnalysisId { get; }
    private Func<IExplorerSession> CreateSession { get; }

    public ObservableCollection<FolderNode> Folders { get; } = new ();

    public FolderNode? SelectedFolderNode
    {
        get => _selectedFolderNode;
        set => SetIfDifferent(ref _selectedFolderNode, value);
    }

    public string Title => "Explorer";

    public async Task LoadFolderNodesAsync()
    {
        await using var session = CreateSession();
        var allFolders = await session.GetAllFoldersAsync(AnalysisId);
        var (rootNode, nodesLookup) = CreateNodesFromEntries(allFolders);
        ConnectFolderNodes(rootNode, nodesLookup);
        Folders.Add(rootNode);
        SelectedFolderNode = rootNode;
    }

    private (FolderNode rootNode, Dictionary<string, FolderNode> nodesLookup) CreateNodesFromEntries(List<FileSystemEntry> folderEntries)
    {
        folderEntries.MustNotBeNullOrEmpty();
        
        var lookup = new Dictionary<string, FolderNode>();
        FolderNode? rootNode = null;

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
            throw new InvalidStateException($"The loaded folder entries do not contain the root node for analysis \"{AnalysisId}\"");

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
}