using FileSystemAnalyzer.AvaloniaApp.Shared;
using Humanizer;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Files;

public sealed class GroupByFileExtensionViewModel
{
    public GroupByFileExtensionViewModel(GroupByFileExtension group)
    {
        Group = group;
        NumberOfFiles = "file".ToQuantity(group.NumberOfFiles);
        TotalSize = group.TotalSizeInBytes.ConvertToDisplaySize();
    }

    public GroupByFileExtension Group { get; }
    public string FileExtension => Group.FileExtension;
    public string NumberOfFiles { get; }
    public string TotalSize { get; }
}