using System.Net;
using Claims.Domain;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public class CosmosDbClaimsRepository : CosmosDbRepository, IClaimsRepository
{
    public CosmosDbClaimsRepository(CosmosClient dbClient, string databaseName, string containerName)
        : base(dbClient, databaseName, containerName)
    {
    }

    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        return await GetAllClaimsAsync();
    }

    private async Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        var query = Container.GetItemQueryIterator<Claim>(new QueryDefinition("SELECT * FROM c"));
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
            var response = await Container.ReadItemAsync<Claim>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
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