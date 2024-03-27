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
        var query = Container.GetItemQueryIterator<Cover>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<Cover>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }
        return results;
    }

    public Task<ItemResponse<Cover>> DeleteCoverAsync(string id)
    {
        return Container.DeleteItemAsync<Cover>(id, new(id));
    }
}
