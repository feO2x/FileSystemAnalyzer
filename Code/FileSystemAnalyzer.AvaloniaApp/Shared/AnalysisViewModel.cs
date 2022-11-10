using System.IO;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using Humanizer;
using Light.GuardClauses;
using Material.Icons;

namespace FileSystemAnalyzer.AvaloniaApp.Shared;

public sealed class AnalysisViewModel
{
    public AnalysisViewModel(Analysis analysis)
    {
        Analysis = analysis;
        Name = Path.GetFileName(analysis.DirectoryPath);
        CreatedAt = analysis.CreatedAtUtc.ConvertToLocalDisplayTime();
        Duration = analysis.CreatedAtUtc.CalculateDuration(analysis.FinishedAtUtc);
        Size = analysis.SizeInBytes.ConvertToDisplaySize();
        NumberOfFolders = "folder".ToQuantity(analysis.NumberOfFolders);
        NumberOfFiles = "file".ToQuantity(analysis.NumberOfFiles);
    }

    public Analysis Analysis { get; }
    public string Name { get; }
    public string DirectoryPath => Analysis.DirectoryPath;
    public string CreatedAt { get; }
    public string Duration { get; }
    public string Size { get; }
    public string NumberOfFolders { get; }
    public string NumberOfFiles { get; }
    public bool IsErroneous => !Analysis.ErrorMessage.IsNullOrWhiteSpace();
    public string Error => Analysis.ErrorMessage ?? string.Empty;

    public MaterialIconKind IconKind =>
        IsErroneous ? MaterialIconKind.FolderCancel : MaterialIconKind.FolderFile;
}