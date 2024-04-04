using Claims.Api.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Claims.Api.Tests;

// ReSharper disable once UnusedMember.Global
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
        DeleteCosmosDbIfExists();
    }

    private static void DeleteCosmosDbIfExists()
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

    private static CosmosDbConfiguration CosmosDbConfiguration()
    {
        return AppConfiguration.FromConfiguration(
            // ReSharper disable once StringLiteralTypo
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
        ).CosmosDb;
    }
}
