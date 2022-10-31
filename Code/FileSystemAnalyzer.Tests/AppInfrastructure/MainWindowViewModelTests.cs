using FileSystemAnalyzer.AvaloniaApp;
using FileSystemAnalyzer.AvaloniaApp.AppInfrastructure;
using FileSystemAnalyzer.AvaloniaApp.Navigation;
using FluentAssertions;
using Xunit;

namespace FileSystemAnalyzer.Tests.AppInfrastructure;

public sealed class MainWindowViewModelTests
{

    private MainWindowViewModel MainWindowViewModel { get; } = new ();
    
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
}