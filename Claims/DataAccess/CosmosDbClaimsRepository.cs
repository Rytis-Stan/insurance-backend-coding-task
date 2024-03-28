using Claims.Domain;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public class CosmosDbClaimsRepository : CosmosDbRepository, IClaimsRepository
{
    public CosmosDbClaimsRepository(CosmosClient dbClient, string databaseName, string containerName)
        : base(dbClient, databaseName, containerName)
    {
    }

    public async Task<Claim?> GetClaimAsync(Guid id)
    {
        return await GetByIdAsync<Claim>(id);
    }

    public async Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        return await GetAllAsync<Claim>();
    }

    public async Task<Claim> AddItemAsync(Claim item)
    {
        return await Container.CreateItemAsync(item, new PartitionKey(item.Id));
    }

    public async Task<Claim> DeleteItemAsync(Guid id)
    {
        return await Container.DeleteItemAsync<Claim>(id.ToString(), new PartitionKey(id.ToString()));
    }
}