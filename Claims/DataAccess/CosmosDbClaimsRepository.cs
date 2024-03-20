using System.Net;
using Claims.Domain;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public class CosmosDbClaimsRepository : IClaimsRepository
{
    private readonly Container _container;

    public CosmosDbClaimsRepository(CosmosClient dbClient, string databaseName, string containerName)
    {
        ArgumentNullException.ThrowIfNull(dbClient, nameof(dbClient));
        _container = dbClient.GetContainer(databaseName, containerName);
    }

    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        return await GetAllClaimsAsync();
    }

    private async Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        var query = _container.GetItemQueryIterator<Claim>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<Claim>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }
        return results;
    }

    public async Task<Claim> GetClaimAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Claim>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public Task AddItemAsync(Claim item)
    {
        return _container.CreateItemAsync(item, new PartitionKey(item.Id));
    }

    public Task DeleteItemAsync(string id)
    {
        return _container.DeleteItemAsync<Claim>(id, new PartitionKey(id));
    }
}