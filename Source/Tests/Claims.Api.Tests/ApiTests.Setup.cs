using Claims.Api.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Claims.Api.Tests;

public partial class ApiTests
{
    private readonly HttpClient _client;

    public ApiTests()
    {
        _client = new TestWebApplicationFactory<Program>().CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();

        // TODO: Rewrite tests so that the databases can be deleted only once for all the API tests?
        DeleteCosmosDbIfExists();
        DeleteSqlServerDatabaseIfExists();
    }

    private void DeleteCosmosDbIfExists()
    {
        var configuration = CosmosDbConfiguration();
        var cosmosClient = new CosmosClient(configuration.Account, configuration.Key);
        try
        {
            cosmosClient.GetDatabase(configuration.DatabaseName).DeleteAsync().GetAwaiter().GetResult();
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Database was not found. Do nothing.
        }
    }

    private void DeleteSqlServerDatabaseIfExists()
    {
        // TODO: Implement database deletion logic.
    }

    private CosmosDbConfiguration CosmosDbConfiguration()
    {
        return AppConfiguration.FromConfiguration(
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
        ).CosmosDb;
    }
}
