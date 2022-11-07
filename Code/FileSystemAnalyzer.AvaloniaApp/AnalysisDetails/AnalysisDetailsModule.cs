﻿using LightInject;

namespace FileSystemAnalyzer.AvaloniaApp.AnalysisDetails;

public static class AnalysisDetailsModule
{
    public static IServiceRegistry RegisterAnalysisDetails(this IServiceRegistry container) =>
        container.RegisterSingleton<AnalysisDetailViewFactory>()
                 .RegisterTransient<FileSystemAnalyzer>()
                 .RegisterTransient<IFileSystemAnalysisSession, RavenDbFileSystemAnalysisSession>();
}