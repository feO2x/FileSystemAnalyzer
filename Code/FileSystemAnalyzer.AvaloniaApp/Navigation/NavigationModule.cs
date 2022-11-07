using FileSystemAnalyzer.AvaloniaApp.AnalysesList;
using FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;
using LightInject;

namespace FileSystemAnalyzer.AvaloniaApp.Navigation;

public static class NavigationModule
{
    public static IServiceRegistry RegisterNavigation(this IServiceRegistry container) =>
        container.RegisterSingleton<ICreateNewAnalysisNavigationCommand, CreateNewAnalysisNavigationCommand>()
                 .RegisterSingleton<INavigateToAnalysesListCommand, NavigateToAnalysesListCommand>()
                 .RegisterSingleton<IOpenExistingAnalysisNavigationCommand, OpenExistingAnalysisNavigationCommand>();
}