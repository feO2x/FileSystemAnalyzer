using Avalonia.Layout;
using FileSystemAnalyzer.AvaloniaApp.Navigation;
using Light.GuardClauses;
using Light.ViewModels;

namespace FileSystemAnalyzer.AvaloniaApp.AppShell;

public sealed class MainWindowViewModel : BaseNotifyPropertyChanged, INavigator
{
    private object? _currentContent;

    public MainWindowViewModel(IOperatingSystem operatingSystem) =>
        OperatingSystem = operatingSystem;

    private IOperatingSystem OperatingSystem { get; }

    public object? CurrentContent
    {
        get => _currentContent;
        private set => Set(out _currentContent, value);
    }

    public bool IsAppIconVisible => OperatingSystem.IsWindows();

    public HorizontalAlignment TitleAlignment =>
        OperatingSystem.IsWindows() ? HorizontalAlignment.Left : HorizontalAlignment.Center;

    public void NavigateTo(object newView) =>
        CurrentContent = newView.MustNotBeNull();
}