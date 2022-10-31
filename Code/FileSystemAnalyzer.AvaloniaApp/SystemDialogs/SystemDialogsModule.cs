using LightInject;

namespace FileSystemAnalyzer.AvaloniaApp.SystemDialogs;

public static class DialogsModule
{
    public static IServiceRegistry RegisterSystemDialogs(this IServiceRegistry serviceContainer) =>
        serviceContainer.RegisterSingleton<IFolderDialog, AvaloniaFolderDialog>();
}