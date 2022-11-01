using System;
using LightInject;
using Microsoft.Extensions.Configuration;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Embedded;
using Synnotech.RavenDB;

namespace FileSystemAnalyzer.AvaloniaApp.DataAccess;

public static class RavenDbModule
{
    public static IServiceRegistry RegisterRavenDb(this IServiceRegistry container, IConfiguration configuration)
    {
        var useEmbeddedServer = configuration.GetValue<bool>("database:useEmbeddedServer");
        var databaseName = configuration["database:databaseName"];
        if (useEmbeddedServer)
            StartEmbeddedServerAndRegisterDocumentStore(container, configuration, databaseName);
        else
            RegisterDocumentStoreForExistingServer(container, configuration, databaseName);

        return container.RegisterTransient(f => f.GetInstance<IDocumentStore>().OpenAsyncSession());
    }

    private static void StartEmbeddedServerAndRegisterDocumentStore(IServiceRegistry container, IConfiguration configuration, string databaseName)
    {
        var serverOptions = configuration.GetSection("database").Get<ServerOptions>();
        Environment.ExpandEnvironmentVariables(serverOptions.DataDirectory);
        var embeddedServer = EmbeddedServer.Instance;
        embeddedServer.StartServer(serverOptions);
        container.RegisterInstance(embeddedServer);
        var databaseOptions = new DatabaseOptions(databaseName)
        {
            Conventions = new DocumentConventions().SetIdentityPartsSeparator()
        };
        var documentStore = embeddedServer.GetDocumentStore(databaseOptions);
        container.RegisterInstance(documentStore);
    }

    private static void RegisterDocumentStoreForExistingServer(IServiceRegistry container, IConfiguration configuration, string databaseName)
    {
        var serverUrl = configuration["database:serverUrl"];
        var documentStore = new DocumentStore
        {
            Urls = new[] { serverUrl },
            Database = databaseName,
            Conventions = new DocumentConventions().SetIdentityPartsSeparator()
        }.Initialize();
        container.RegisterInstance(documentStore);
    }
}