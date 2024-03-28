using System.Net;
using Claims.Infrastructure;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public abstract class CosmosDbRepository
{
    protected readonly Container Container;
    protected readonly IClock Clock;

    protected CosmosDbRepository(CosmosClient dbClient, string databaseName, string containerName, IClock clock)
    {
        ArgumentNullException.ThrowIfNull(dbClient, nameof(dbClient));
        Container = dbClient.GetContainer(databaseName, containerName);
        Clock = clock;
    }

    protected async Task<T?> GetByIdAsync<T>(Guid id)
        where T : class
    {
        try
        {
            var response = await Container.ReadItemAsync<T>(id.ToString(), new PartitionKey(id.ToString()));
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
