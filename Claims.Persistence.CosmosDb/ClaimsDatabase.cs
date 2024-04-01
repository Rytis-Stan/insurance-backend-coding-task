using Claims.Infrastructure;
using Claims.Repositories;
using Microsoft.Azure.Cosmos;

namespace Claims.Persistence.CosmosDb;

public class ClaimsDatabase : IClaimsDatabase
{
    private readonly CosmosClient _client;
    private readonly string _name;
    private readonly IIdGenerator _idGenerator;

    public IClaimsRepository ClaimsRepository => new CosmosDbClaimsRepository(_client, _name, _idGenerator);
    public ICoversRepository CoversRepository => new CosmosDbCoversRepository(_client, _name, _idGenerator);

    public ClaimsDatabase(CosmosClient client, string name, IIdGenerator idGenerator)
    {
        _client = client;
        _name = name;
        _idGenerator = idGenerator;
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