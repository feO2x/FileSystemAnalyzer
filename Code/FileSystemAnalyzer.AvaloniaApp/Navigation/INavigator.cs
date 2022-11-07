using FileSystemAnalyzer.AvaloniaApp.AppShell;

namespace FileSystemAnalyzer.AvaloniaApp.Navigation;

public interface INavigator
{
    void NavigateTo(IView newView);
}