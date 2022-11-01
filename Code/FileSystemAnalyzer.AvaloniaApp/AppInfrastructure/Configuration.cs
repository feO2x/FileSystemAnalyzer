using LightInject;
using Microsoft.Extensions.Configuration;

namespace FileSystemAnalyzer.AvaloniaApp.AppInfrastructure;

public static class Configuration
{
    private static IConfiguration LoadConfiguration() =>
        new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false)
                                  .AddJsonFile("appsettings.Development.json", optional: true)
                                  .AddJsonFile("appsettings.Production.json", optional: true)
                                  .Build();

    public static IServiceRegistry RegisterConfiguration(this IServiceRegistry container, out IConfiguration configuration)
    {
        configuration = LoadConfiguration();
        return container.RegisterInstance(configuration);
    }
}