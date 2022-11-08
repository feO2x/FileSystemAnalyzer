using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails.Explorer;

public partial class ExplorerView : UserControl
{
    public ExplorerView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}