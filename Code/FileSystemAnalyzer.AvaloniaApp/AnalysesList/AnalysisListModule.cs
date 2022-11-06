using FileSystemAnalyzer.AvaloniaApp.Shared;
using LightInject;
using Microsoft.Extensions.Configuration;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysesList;

public static class AnalysisListModule
{
    public static IServiceRegistry RegisterAnalysisList(this IServiceRegistry container, IConfiguration configuration)
    {
        container.RegisterViewAndViewModelWithTransientLifetimes<AnalysesListView, AnalysesListViewModel>()
                 .RegisterSingleton<CreateNewAnalysisCommand>();

        if (configuration.GetValue<bool>("analysisSession:useFake"))
        {
            var numberOfItems = configuration.GetValue("analysisSession:numberOfItems", 300);
            var delayInMilliseconds = configuration.GetValue("analysisSession:delayInMilliseconds", 50);
            container.RegisterInstance<IAnalysesSession>(BogusAnalysesSession.Create(numberOfItems, delayInMilliseconds));
        }
        else
        {
            container.RegisterTransient<IAnalysesSession, RavenDbAnalysesSession>();
        }

        return container;
    }
}