using FileSystemAnalyzer.AvaloniaApp.GettingStarted;
using LightInject;

namespace FileSystemAnalyzer.AvaloniaApp.Navigation;

public static class NavigationModule
{
    public static IServiceRegistry RegisterNavigation(this IServiceRegistry container) =>
        container.RegisterSingleton<ICreateNewAnalysisNavigationCommand, CreateNewAnalysisNavigationCommand>()
                 .RegisterSingleton<InitialViewNavigationCommand>();
}