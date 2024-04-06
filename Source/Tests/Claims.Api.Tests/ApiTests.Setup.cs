using Claims.Api.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Claims.Api.Tests;

// This class contains tests for all the API endpoints (the tests themselves are in the
// different segments of this partial class). The tests are not split up into separate
// classes like "CoverControllerTests" and "ClaimsControllerTests" because:
// 1. The purpose is not to test the controllers themselves, but the whole API with all
//    of it's underlying layers;
// 2. It might also make sense to sometimes have dedicated controller tests just for
//    testing the controllers themselves, while injecting mocked dependencies into them
//    (so naming the API tests as multiple classes of "SomeControllerTests" would cause
//    confusion with the naming);
// 3. The API tests should not care that the endpoints are exposed via controllers. The
//    underling API technology should be free to change without affecting the tests much.
//    Ideally, these tests could relatively easily be redone to connect to an externally
//    hosted API, if such a need would arise (so there would be no usage of things like
//    "WebApplicationFactory" and the whole test project could not even reference the
//    main API project; the only problem would be with the DTO reuse, for which the DTOs
//    could be moved out into a separate project that is referenced both by the API and
//    the API tests projects).
//
// This partial class segment is dedicated to the setup and teardown code of all the tests.
//
// ReSharper disable once UnusedMember.Global
public partial class ApiTests
{
    private readonly HttpClient _client;

    public ApiTests()
    {
        DeleteCosmosDbIfExists();
        _client = new TestWebApplicationFactory<Program>().CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
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
            // Database was not found.
        }
    }

    private static CosmosDbConfiguration CosmosDbConfiguration()
    {
        return new AppConfiguration(
            // ReSharper disable once StringLiteralTypo
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
        ).CosmosDb;
    }
}
