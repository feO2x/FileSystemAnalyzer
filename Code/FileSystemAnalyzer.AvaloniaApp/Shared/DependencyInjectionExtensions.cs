using Avalonia;
using LightInject;

namespace FileSystemAnalyzer.AvaloniaApp.Shared;

public static class DependencyInjectionExtensions
{
    public static IServiceRegistry RegisterViewAndViewModelWithTransientLifetimes<TView, TViewModel>(this IServiceRegistry container)
        where TView : StyledElement, new() =>
        container.RegisterTransient<TViewModel>()
                 .RegisterTransient(f => new TView { DataContext = f.GetInstance<TViewModel>() });
    
    public static IServiceRegistry RegisterViewAndViewModelAsSingletons<TView, TViewModel>(this IServiceRegistry container)
        where TView : StyledElement, new() =>
        container.RegisterSingleton<TViewModel>()
                 .RegisterSingleton(f => new TView { DataContext = f.GetInstance<TViewModel>() });
}