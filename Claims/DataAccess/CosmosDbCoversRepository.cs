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

    public async Task<ItemResponse<Cover>> AddItemAsync(Cover cover)
    {
        return await Container.CreateItemAsync(cover, new PartitionKey(cover.Id));
    }

    public Task<ItemResponse<Cover>> DeleteItemAsync(string id)
    {
        return Container.DeleteItemAsync<Cover>(id, new(id));
    }
}
