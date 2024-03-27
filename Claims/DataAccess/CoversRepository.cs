using System.Net;
using Claims.Domain;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public class CoversRepository : ICoversRepository
{
    private readonly Container _container;

    public CoversRepository(Container container)
    {
        _container = container;
    }

    public async Task<ItemResponse<Cover>> AddItemAsync(Cover cover)
    {
        return await _container.CreateItemAsync(cover, new PartitionKey(cover.Id));
    }

    public async Task<Cover?> GetCoverAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Cover>(id, new(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Cover>> GetAllCoversAsync()
    {
        var query = _container.GetItemQueryIterator<Cover>(new QueryDefinition("SELECT * FROM c"));
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
        return _container.DeleteItemAsync<Cover>(id, new(id));
    }
}
