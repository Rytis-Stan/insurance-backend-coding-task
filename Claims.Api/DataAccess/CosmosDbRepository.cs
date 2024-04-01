using System.Net;
using Claims.Infrastructure;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public abstract class CosmosDbRepository<T, TNewItemInfo, TJson>
    where T : class
    where TNewItemInfo : class
    where TJson : class
{
    private readonly Container _container;
    private readonly IIdGenerator _idGenerator;

    protected CosmosDbRepository(CosmosClient dbClient, string databaseName, string containerName, IIdGenerator idGenerator)
    {
        ArgumentNullException.ThrowIfNull(dbClient, nameof(dbClient));
        _container = dbClient.GetContainer(databaseName, containerName);
        _idGenerator = idGenerator;
    }

    public async Task<T> AddAsync(TNewItemInfo item)
    {
        var id = _idGenerator.NewId().ToString();
        var json = ToNewJson(id, item);
        return ToItem(await _container.CreateItemAsync(json, new PartitionKey(id)));
    }

    public async Task<T?> FindByIdAsync(Guid id)
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

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<TJson>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.Select(ToItem));
        }
        return results;
    }

    public async Task<T?> DeleteAsync(Guid id)
    {
        return await DeleteAsync(id.ToString());
    }

    private async Task<T?> DeleteAsync(string id)
    {
        var batch = _container.CreateTransactionalBatch(new PartitionKey(id));
        batch.ReadItem(id);
        batch.DeleteItem(id);
        var response = await batch.ExecuteAsync();

        var json = response.GetOperationResultAtIndex<TJson>(0).Resource;
        return json != null
            ? ToItem(json)
            : null;
    }

    protected abstract TJson ToNewJson(string id, TNewItemInfo item);
    protected abstract T ToItem(TJson json);
}
