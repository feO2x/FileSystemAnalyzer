using FileSystemAnalyzer.AvaloniaApp.GettingStarted;
using FileSystemAnalyzer.Tests.SystemDialogs;
using FluentAssertions;
using Xunit;

namespace FileSystemAnalyzer.Tests.GettingStarted;

public sealed class InitialViewModelTests
{
    public InitialViewModelTests()
    {
        FolderDialog = new ();
        NavigationCommand = new ();
        InitialViewModel = new (FolderDialog, NavigationCommand);
    }
    
    private FolderDialogStub FolderDialog { get; }
    private NavigationCommandSpy NavigationCommand { get; }
    private InitialViewModel InitialViewModel { get; }

    [Fact]
    public void NavigateAfterTargetDirectoryWasChosen()
    {
        FolderDialog.ReturnValue = @"C:\SomeDirectory";
        
        InitialViewModel.AnalyzeFolderCommand.Execute();

        NavigationCommand.GetTargetDirectoryPath().Should().BeSameAs(FolderDialog.ReturnValue);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DoNotNavigateIfUserCancelledDialog(string? dialogReturnValue)
    {
        FolderDialog.ReturnValue = dialogReturnValue;
        
        InitialViewModel.AnalyzeFolderCommand.Execute();
        
        NavigationCommand.NavigateMustNotHaveBeenCalled();
    }
    
    private sealed class NavigationCommandSpy : ICreateNewAnalysisNavigationCommand
    {
        private List<string> CapturedParameters { get; } = new ();

        public void Navigate(string targetDirectoryPath)
        {
            CapturedParameters.Add(targetDirectoryPath);
        }

        public string GetTargetDirectoryPath()
        {
            CapturedParameters.Should().HaveCount(1);
            return CapturedParameters[0];
        }

        public void NavigateMustNotHaveBeenCalled() =>
            CapturedParameters.Should().BeEmpty();
    }
}

