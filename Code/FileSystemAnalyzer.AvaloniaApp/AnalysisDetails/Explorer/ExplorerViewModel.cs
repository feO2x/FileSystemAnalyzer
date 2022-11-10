using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.EndlessScrolling;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Light.ViewModels;
using Serilog;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Explorer;

public sealed class ExplorerViewModel : BaseNotifyPropertyChanged,
                                        IHasPagingViewModel,
                                        IConverter<FileSystemEntry, FileSystemEntryViewModel>,
                                        ITabItemViewModel
{
    private FolderNode? _selectedFolderNode;

    public ExplorerViewModel(string analysisId,
                             Func<IExplorerSession> createSession,
                             DebouncedValueFactory debouncedValueFactory,
                             ILogger logger)
    {
        AnalysisId = analysisId;
        CreateSession = createSession;
        PagingViewModel = new (createSession, 100, new (), this, logger);
        DebouncedSearchTerm = debouncedValueFactory.CreateDebouncedValue(string.Empty, OnDebouncedSearchTermChanged);
    }

    private string AnalysisId { get; }
    private Func<IExplorerSession> CreateSession { get; }
    public ObservableCollection<FolderNode> Folders { get; } = new ();

    public FolderNode? SelectedFolderNode
    {
        get => _selectedFolderNode;
        set
        {
            if (SetIfDifferent(ref _selectedFolderNode, value) && value is not null)
                PagingViewModel.Filters = PagingViewModel.Filters with { ParentFolderId = value.FolderEntry.Id };
        }
    }

    private DebouncedValue<string> DebouncedSearchTerm { get; }

    public string SearchTerm
    {
        get => DebouncedSearchTerm.CurrentValue;
        set
        {
            if (DebouncedSearchTerm.TrySetValue(value))
                OnPropertyChanged();
        }
    }

    public PagingViewModel<FileSystemEntryViewModel, FileSystemEntry, ExplorerFilters> PagingViewModel { get; }

    private Dictionary<string, FolderNode> NodeLookup { get; } = new ();

    FileSystemEntryViewModel IConverter<FileSystemEntry, FileSystemEntryViewModel>.Convert(FileSystemEntry value) => new (value);
    IPagingViewModel IHasPagingViewModel.PagingViewModel => PagingViewModel;

    public string Title => "Explorer";

    void ITabItemViewModel.Reload(bool isOptional)
    {
        if (!isOptional)
            PagingViewModel.ReloadAsync(true);
    }

    public async Task LoadFolderNodesAsync()
    {
        await using var session = CreateSession();
        var allFolders = await session.GetAllFoldersAsync(AnalysisId);
        var rootNode = FolderNode.CreateTreeFromFlatFolderEntries(allFolders, NodeLookup);
        Folders.Add(rootNode);
        SelectedFolderNode = rootNode;
    }

    private void OnDebouncedSearchTermChanged() =>
        PagingViewModel.Filters = PagingViewModel.Filters with { SearchTerm = DebouncedSearchTerm.CurrentValue };

    public void InsertFolder(FileSystemEntry newFolder)
    {
        var node = new FolderNode(newFolder);
        if (newFolder.ParentId is null)
        {
            Folders.Add(node);
        }
        else
        {
            var currentParentNode = NodeLookup[newFolder.ParentId];
            currentParentNode.AddChildNode(node);
            currentParentNode.RaiseSizeChanged();
            while (currentParentNode.FolderEntry.ParentId is not null)
            {
                currentParentNode = NodeLookup[currentParentNode.FolderEntry.ParentId];
                currentParentNode.RaiseSizeChanged();
            }
        }

        NodeLookup.Add(newFolder.Id, node);
    }

    public void SelectFolder(FileSystemEntry fileSystemEntry)
    {
        if (fileSystemEntry.Type != FileSystemEntryType.Folder)
            throw new ArgumentException("The specified entry must be a folder entry", nameof(fileSystemEntry));

        
        var currentNode = SelectedFolderNode = NodeLookup[fileSystemEntry.Id];
        while (currentNode.Parent is not null)
        {
            currentNode = currentNode.Parent;
            currentNode.IsExpanded = true;
        }
    }
}