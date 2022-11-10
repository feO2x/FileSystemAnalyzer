using Humanizer.Configuration;
using Humanizer.DateTimeHumanizeStrategy;

namespace FileSystemAnalyzer.AvaloniaApp.AppInfrastructure;

public static class HumanizerConfiguration
{
    public static void ConfigureHumanizer() =>
        Configurator.DateTimeHumanizeStrategy = new PrecisionDateTimeHumanizeStrategy(0.9);
}