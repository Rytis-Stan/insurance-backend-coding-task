using System.Net;
using Microsoft.Azure.Cosmos;

namespace Claims.Persistence;

public abstract class CosmosDbRepository<TNewObjectInfo, TObject, TJson>
    where TNewObjectInfo : class
    where TObject : class
    where TJson : class
{
    private readonly Container _container;
    private readonly IIdSource _idSource;

    protected CosmosDbRepository(CosmosClient dbClient, string databaseName, string containerName, IIdSource idSource)
    {
        ArgumentNullException.ThrowIfNull(dbClient, nameof(dbClient));
        _container = dbClient.GetContainer(databaseName, containerName);
        _idSource = idSource;
    }

    public async Task<TObject> CreateAsync(TNewObjectInfo newObject)
    {
        var id = _idSource.NewId().ToString();
        var json = ToNewJson(id, newObject);
        return ToItem(await _container.CreateItemAsync(json, new PartitionKey(id)));
    }

    public async Task<TObject?> FindByIdAsync(Guid id)
    {
        try
        {
            var response = await _container.ReadItemAsync<TJson>(id.ToString(), new PartitionKey(id.ToString()));
            return ToItem(response.Resource);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<TObject>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<TJson>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<TObject>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.Select(ToItem));
        }
        return results;
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        await DeleteByIdAsync(id.ToString());
    }

    private async Task DeleteByIdAsync(string id)
    {
        try
        {
            await _container.DeleteItemAsync<TJson>(id, new PartitionKey(id));
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Item not found in the database. Let's assume that it's ok to do a delete
            // operation for a non-existing item. No further processing needed.
        }
    }

    protected abstract TJson ToNewJson(string id, TNewObjectInfo item);
    protected abstract TObject ToItem(TJson json);
}
