using System.Net;
using Claims.Domain;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public class CosmosDbCoversRepository : CosmosDbRepository, ICoversRepository
{
    public CosmosDbCoversRepository(CosmosClient dbClient, string databaseName, string containerName)
        : base(dbClient, databaseName, containerName)
    {
    }

    public async Task<ItemResponse<Cover>> AddItemAsync(Cover cover)
    {
        return await Container.CreateItemAsync(cover, new PartitionKey(cover.Id));
    }

    public async Task<Cover?> GetCoverAsync(string id)
    {
        try
        {
            var response = await Container.ReadItemAsync<Cover>(id, new(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Cover>> GetAllCoversAsync()
    {
        return await GetAllAsync<Cover>();
    }

    public Task<ItemResponse<Cover>> DeleteItemAsync(string id)
    {
        return Container.DeleteItemAsync<Cover>(id, new(id));
    }
}
