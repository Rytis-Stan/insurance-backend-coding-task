using System.Net;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public abstract class CosmosDbRepository
{
    protected readonly Container Container;

    protected CosmosDbRepository(CosmosClient dbClient, string databaseName, string containerName)
    {
        ArgumentNullException.ThrowIfNull(dbClient, nameof(dbClient));
        Container = dbClient.GetContainer(databaseName, containerName);
    }

    protected async Task<T?> GetByIdAsync<T>(string id)
        where T : class
    {
        try
        {
            var response = await Container.ReadItemAsync<T>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    protected async Task<IEnumerable<T>> GetAllAsync<T>()
    {
        var query = Container.GetItemQueryIterator<T>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }
        return results;
    }
}
