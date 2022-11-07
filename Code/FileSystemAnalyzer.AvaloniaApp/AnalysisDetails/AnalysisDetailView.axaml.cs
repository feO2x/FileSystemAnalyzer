using System.IO;
using Avalonia.Controls;
using FileSystemAnalyzer.AvaloniaApp.AppShell;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

public sealed partial class AnalysisDetailView : UserControl, IView, INavigateBack
{
    public AnalysisDetailView() => InitializeComponent();

    public string Title
    {
        get
        {
            if (DataContext is AnalysisDetailViewModel viewModel)
                return Path.GetFileName(viewModel.Analysis.DirectoryPath);

            return "Analysis Details";
        }
    }

    public void NavigateBack()
    {
        if (DataContext is AnalysisDetailViewModel viewModel)
            viewModel.NavigateBack();
    }
}