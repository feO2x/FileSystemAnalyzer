using System;
using FileSystemAnalyzer.AvaloniaApp.AppShell;
using FileSystemAnalyzer.AvaloniaApp.Navigation;
using FluentAssertions;
using Xunit;

namespace FileSystemAnalyzer.Tests.AppShell;

public sealed class MainWindowViewModelTests
{
    public MainWindowViewModelTests()
    {
        OperatingSystem = new ();
        MainWindowViewModel = new (OperatingSystem);
    }
    
    private OperatingSystemStub OperatingSystem { get; }
    private MainWindowViewModel MainWindowViewModel { get; }
    
    [Fact]
    public static void MustImplementINavigator() =>
        typeof(MainWindowViewModel).Should().Implement<INavigator>();

    [Fact]
    public void SetNewViewToValidObject()
    {
        var newView = new object();
        using var eventMonitor = MainWindowViewModel.Monitor();
        
        MainWindowViewModel.NavigateTo(newView);

        MainWindowViewModel.CurrentContent.Should().BeSameAs(newView);
        eventMonitor.Should().RaisePropertyChangeFor(vm => vm.CurrentContent);
    }

    [Fact]
    public void MustThrowWhenNewViewIsNull()
    {
        var act = () => MainWindowViewModel.NavigateTo(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AppIconIsVisibleOnWindows()
    {
        OperatingSystem.IsWindowsReturnValue = true;
        
        MainWindowViewModel.IsAppIconVisible.Should().BeTrue();
    }

    [Fact]
    public void AppIconIsNotVisibleOnOtherOperatingSystems()
    {
        OperatingSystem.IsWindowsReturnValue = false;

        MainWindowViewModel.IsAppIconVisible.Should().BeFalse();
    }
    
    private sealed class OperatingSystemStub : IOperatingSystem
    {
        public bool IsWindowsReturnValue { get; set; }
        
        public bool IsWindows() => IsWindowsReturnValue;
    }
}