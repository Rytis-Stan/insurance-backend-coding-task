using Claims.Domain;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public class CosmosDbCoversRepository : CosmosDbRepository, ICoversRepository
{
    public CosmosDbCoversRepository(CosmosClient dbClient, string databaseName, string containerName)
        : base(dbClient, databaseName, containerName)
    {
    }

    public async Task<Cover?> GetCoverAsync(string id)
    {
        return await GetByIdAsync<Cover>(id);
    }

    public async Task<IEnumerable<Cover>> GetAllCoversAsync()
    {
        return await GetAllAsync<Cover>();
    }

    public async Task<Cover> AddItemAsync(Cover item)
    {
        return await Container.CreateItemAsync(item, new PartitionKey(item.Id));
    }

    public async Task<Cover> DeleteItemAsync(string id)
    {
        return await Container.DeleteItemAsync<Cover>(id, new PartitionKey(id));
    }
}
