using Avalonia.Layout;
using FileSystemAnalyzer.AvaloniaApp.Navigation;
using Light.GuardClauses;
using Light.ViewModels;

namespace FileSystemAnalyzer.AvaloniaApp.AppShell;

public sealed class MainWindowViewModel : BaseNotifyPropertyChanged, INavigator
{
    private IView? _currentView;

    public MainWindowViewModel(IOperatingSystem operatingSystem)
    {
        OperatingSystem = operatingSystem;
        NavigateBackCommand = new (NavigateBack);
    }

    private IOperatingSystem OperatingSystem { get; }

    public IView? CurrentView
    {
        get => _currentView;
        private set
        {
            Set(out _currentView, value);
            OnPropertyChanged(nameof(CurrentTitle));
            OnPropertyChanged(nameof(IsNavigateBackButtonVisible));
        }
    }

    public string? CurrentTitle => CurrentView?.Title;
    public bool IsNavigateBackButtonVisible => CurrentView is INavigateBack;
    public DelegateCommand NavigateBackCommand { get; }
    public bool IsWindowsOperatingSystem => OperatingSystem.IsWindows();

    public HorizontalAlignment TitleAlignment =>
        OperatingSystem.IsWindows() ? HorizontalAlignment.Left : HorizontalAlignment.Center;

    public void NavigateTo(IView newView) =>
        CurrentView = newView.MustNotBeNull();

    private void NavigateBack()
    {
        if (CurrentView is INavigateBack navigateBackView)
            navigateBackView.NavigateBack();
    }
}