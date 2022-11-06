using System.Collections.Generic;
using FileSystemAnalyzer.AvaloniaApp.AnalysesList;
using FileSystemAnalyzer.Tests.SystemDialogs;
using FluentAssertions;
using Xunit;

namespace FileSystemAnalyzer.Tests.AnalysesList;

public sealed class CreateNewAnalysisCommandTests
{
    public CreateNewAnalysisCommandTests()
    {
        FolderDialog = new ();
        NavigationCommand = new ();
        Command = new (FolderDialog, NavigationCommand);
    }
    
    private FolderDialogStub FolderDialog { get; }
    private NavigationCommandSpy NavigationCommand { get; }
    private CreateNewAnalysisCommand Command { get; }

    [Fact]
    public void NavigateAfterTargetDirectoryWasChosen()
    {
        FolderDialog.ReturnValue = @"C:\SomeDirectory";
        
        Command.Execute();

        NavigationCommand.GetTargetDirectoryPath().Should().BeSameAs(FolderDialog.ReturnValue);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void DoNotNavigateIfUserCancelledDialog(string? dialogReturnValue)
    {
        FolderDialog.ReturnValue = dialogReturnValue;
        
        Command.Execute();
        
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

