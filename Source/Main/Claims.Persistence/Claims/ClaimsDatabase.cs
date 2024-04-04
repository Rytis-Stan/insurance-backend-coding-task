using Claims.Application.Repositories;
using Microsoft.Azure.Cosmos;

namespace Claims.Persistence.Claims;

public class ClaimsDatabase : IClaimsDatabase
{
    private readonly CosmosClient _client;
    private readonly string _name;
    private readonly IIdSource _idSource;

    public IClaimsRepository ClaimsRepository => new CosmosDbClaimsRepository(_client, _name, _idSource);
    public ICoversRepository CoversRepository => new CosmosDbCoversRepository(_client, _name, _idSource);

    public ClaimsDatabase(CosmosClient client, string name, IIdSource idSource)
    {
        _client = client;
        _name = name;
        _idSource = idSource;
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