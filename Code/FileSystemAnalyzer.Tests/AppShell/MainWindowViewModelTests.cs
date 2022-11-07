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
        var newView = new DummyView();
        using var eventMonitor = MainWindowViewModel.Monitor();
        
        MainWindowViewModel.NavigateTo(newView);

        MainWindowViewModel.CurrentView.Should().BeSameAs(newView);
        eventMonitor.Should().RaisePropertyChangeFor(vm => vm.CurrentView);
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
        
        MainWindowViewModel.IsWindowsOperatingSystem.Should().BeTrue();
    }

    [Fact]
    public void AppIconIsNotVisibleOnOtherOperatingSystems()
    {
        OperatingSystem.IsWindowsReturnValue = false;

        MainWindowViewModel.IsWindowsOperatingSystem.Should().BeFalse();
    }
    
    private sealed class OperatingSystemStub : IOperatingSystem
    {
        public bool IsWindowsReturnValue { get; set; }
        
        public bool IsWindows() => IsWindowsReturnValue;
    }
    
    private sealed class DummyView : IView
    {
        public string Title => "Dummy View";
    }
}