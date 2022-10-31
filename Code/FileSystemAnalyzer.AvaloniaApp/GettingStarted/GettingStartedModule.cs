using FileSystemAnalyzer.AvaloniaApp.Shared;
using LightInject;

namespace FileSystemAnalyzer.AvaloniaApp.GettingStarted;

public static class GettingStartedModule
{
    public static IServiceRegistry RegisterGettingStarted(this IServiceRegistry container) =>
        container.RegisterViewAndViewModelWithTransientLifetimes<InitialView, InitialViewModel>();
}