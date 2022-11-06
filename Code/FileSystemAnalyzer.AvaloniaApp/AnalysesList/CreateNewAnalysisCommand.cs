using FileSystemAnalyzer.AvaloniaApp.Shared;
using FileSystemAnalyzer.AvaloniaApp.SystemDialogs;
using Light.GuardClauses;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

public sealed class CreateNewAnalysisCommand : BaseCommand
{
    public CreateNewAnalysisCommand(IFolderDialog folderDialog,
                                    ICreateNewAnalysisNavigationCommand navigationCommand)
    {
        FolderDialog = folderDialog;
        NavigationCommand = navigationCommand;
    }

    private IFolderDialog FolderDialog { get; }
    private ICreateNewAnalysisNavigationCommand NavigationCommand { get; }

    public override async void Execute()
    {
        var chosenFolder = await FolderDialog.ChooseFolderAsync();
        if (!chosenFolder.IsNullOrWhiteSpace())
            NavigationCommand.Navigate(chosenFolder);
    }
}