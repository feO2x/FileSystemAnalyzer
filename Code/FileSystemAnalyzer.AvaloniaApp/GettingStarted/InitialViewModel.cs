using FileSystemAnalyzer.AvaloniaApp.SystemDialogs;
using Light.GuardClauses;
using Light.ViewModels;

namespace FileSystemAnalyzer.AvaloniaApp.GettingStarted;

public sealed class InitialViewModel
{
    public InitialViewModel(IFolderDialog folderDialog,
                            ICreateNewAnalysisNavigationCommand createNewAnalysisNavigationCommand)
    {
        FolderDialog = folderDialog;
        CreateNewAnalysisNavigationCommand = createNewAnalysisNavigationCommand;
        AnalyzeFolderCommand = new (AnalyzeFolder);
    }

    private IFolderDialog FolderDialog { get; }
    private ICreateNewAnalysisNavigationCommand CreateNewAnalysisNavigationCommand { get; }
    public DelegateCommand AnalyzeFolderCommand { get; }

    private async void AnalyzeFolder()
    {
        var chosenFolder = await FolderDialog.ChooseFolderAsync();
        if (!chosenFolder.IsNullOrWhiteSpace())
            CreateNewAnalysisNavigationCommand.Navigate(chosenFolder);
    }
}