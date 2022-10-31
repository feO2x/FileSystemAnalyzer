using FileSystemAnalyzer.AvaloniaApp.Navigation;
using Light.GuardClauses;
using Light.ViewModels;

namespace FileSystemAnalyzer.AvaloniaApp.AppInfrastructure;

public sealed class MainWindowViewModel : BaseNotifyPropertyChanged, INavigator
{
    private object? _currentContent;

    public object? CurrentContent
    {
        get => _currentContent;
        private set => Set(out _currentContent, value);
    }

    public void NavigateTo(object newView) =>
        CurrentContent = newView.MustNotBeNull();
}