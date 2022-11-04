using FileSystemAnalyzer.AvaloniaApp.Shared;
using LightInject;
using Microsoft.Extensions.Configuration;

namespace FileSystemAnalyzer.AvaloniaApp.GettingStarted;

public static class GettingStartedModule
{
    public static IServiceRegistry RegisterGettingStarted(this IServiceRegistry container, IConfiguration configuration)
    {
        container.RegisterViewAndViewModelWithTransientLifetimes<InitialView, InitialViewModel>()
                 .RegisterTransient<ExistingAnalysesViewModel>();

        if (configuration.GetValue<bool>("useBogusAnalysesSession"))
        {
            container.RegisterTransient<IAnalysesSession, BogusAnalysesSession>();
            container.RegisterInstance(BogusAnalysesSession.CreateFaker());
        }
        else
        {
            container.RegisterTransient<IAnalysesSession, RavenDbAnalysesSession>();
        }

        return container;
    }
}