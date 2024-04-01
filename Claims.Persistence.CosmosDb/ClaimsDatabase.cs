using Microsoft.Azure.Cosmos;

namespace Claims.Persistence.CosmosDb;

public class ClaimsDatabase
{
    private readonly CosmosClient _client;
    private readonly string _name;

    public ClaimsDatabase(CosmosClient client, string name)
    {
        _client = client;
        _name = name;
    }

    public async Task InitializeAsync()
    {
        var response = await _client.CreateDatabaseIfNotExistsAsync(_name);
        foreach (var containerName in ContainerNames.All)
        {
            await response.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
        }
    }
}
