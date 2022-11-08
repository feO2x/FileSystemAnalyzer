using System.IO;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.Shared;
using Humanizer;
using Light.GuardClauses;
using Material.Icons;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

public sealed class AnalysisViewModel
{
    public AnalysisViewModel(Analysis analysis)
    {
        Analysis = analysis;
        Name = Path.GetFileName(analysis.DirectoryPath);
        CreatedAt = analysis.CreatedAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
        Size = analysis.SizeInBytes.ConvertToDisplaySize();
        NumberOfFolders = "folder".ToQuantity(analysis.NumberOfFolders);
        NumberOfFiles = "file".ToQuantity(analysis.NumberOfFiles);
    }

    public Analysis Analysis { get; }
    public string Name { get; }
    public string DirectoryPath => Analysis.DirectoryPath;
    public string CreatedAt { get; }
    public string Size { get; }
    public string NumberOfFolders { get; }
    public string NumberOfFiles { get; }
    public bool IsErroneous => !Analysis.ErrorMessage.IsNullOrWhiteSpace();

    public MaterialIconKind IconKind =>
        IsErroneous ? MaterialIconKind.FolderCancel : MaterialIconKind.FolderFile;
}