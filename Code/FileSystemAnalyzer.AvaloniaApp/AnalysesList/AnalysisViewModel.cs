using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using FileSystemAnalyzer.AvaloniaApp.DataAccess.Model;
using FileSystemAnalyzer.AvaloniaApp.Shared;
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
        Size = analysis.SizeInBytes.FormatSize();
    }
    
    public Analysis Analysis { get; }
    public string Name { get; }
    public string DirectoryPath => Analysis.DirectoryPath;
    public string CreatedAt { get; }
    public string Size { get; }
    public bool IsErroneous => !Analysis.ErrorMessage.IsNullOrWhiteSpace();
    public MaterialIconKind IconKind =>
        IsErroneous ? MaterialIconKind.FolderCancel : MaterialIconKind.FolderFile;
}

public static class AnalysisViewModelExtensions
{
    public static void AppendAsViewModels(this ObservableCollection<AnalysisViewModel> viewModels, List<Analysis> entities)
    {
        for (var i = 0; i < entities.Count; i++)
        {
            var viewModel = new AnalysisViewModel(entities[i]);
            viewModels.Add(viewModel);
        }
    }
}