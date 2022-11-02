using System;
using LightInject;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace FileSystemAnalyzer.AvaloniaApp.AppInfrastructure;

public static class Logging
{
    private static LoggingLevelSwitch LoggingLevelSwitch { get; } = new ();
    private static ILogger? Logger { get; set; }
    
    public static IServiceRegistry RegisterLogger(this IServiceRegistry container, IConfiguration configuration)
    {
        var logFilePath = configuration["logging:logFilePath"];
        logFilePath = Environment.ExpandEnvironmentVariables(logFilePath);

        var logLevelText = configuration["logging:logLevel"];
        if (Enum.TryParse<LogEventLevel>(logLevelText, true, out var parsedLogLevel))
            LoggingLevelSwitch.MinimumLevel = parsedLogLevel;
        
        var logger = new LoggerConfiguration().MinimumLevel.ControlledBy(LoggingLevelSwitch)
                                              .WriteTo.File(logFilePath)
                                              .CreateLogger();
        Logger = logger;
        container.RegisterInstance<ILogger>(logger);
        return container;
    }

    public static void LogFatalError(Exception exception)
    {
        var logger = Logger ?? new LoggerConfiguration().WriteTo.File("StartupError.log")
                                                        .CreateLogger();
        logger.Fatal(exception, "Fatal error occurred");
    }
}