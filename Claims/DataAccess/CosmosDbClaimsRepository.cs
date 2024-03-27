using Claims.Domain;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public class CosmosDbClaimsRepository : CosmosDbRepository, IClaimsRepository
{
    public CosmosDbClaimsRepository(CosmosClient dbClient, string databaseName, string containerName)
        : base(dbClient, databaseName, containerName)
    {
    }

    public async Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        return await GetAllAsync<Claim>();
    }

    public async Task<Claim?> GetClaimAsync(string id)
    {
        return await GetByIdAsync<Claim>(id);
    }

    public Task AddItemAsync(Claim item)
    {
        return Container.CreateItemAsync(item, new PartitionKey(item.Id));
    }

    public Task DeleteItemAsync(string id)
    {
        return Container.DeleteItemAsync<Claim>(id, new PartitionKey(id));
    }
}