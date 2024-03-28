using System.Net;
using Claims.Infrastructure;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public abstract class CosmosDbRepository<T, TNewItemInfo, TJson>
    where T : class
    where TNewItemInfo : class
    where TJson : class, IHasStringId
{
    protected readonly Container Container;
    protected readonly IClock Clock;
    private readonly IIdGenerator _idGenerator;

    protected CosmosDbRepository(CosmosClient dbClient, string databaseName, string containerName, IClock clock, IIdGenerator idGenerator)
    {
        ArgumentNullException.ThrowIfNull(dbClient, nameof(dbClient));
        Container = dbClient.GetContainer(databaseName, containerName);
        Clock = clock;
        _idGenerator = idGenerator;
    }

    public async Task<T> AddAsync(TNewItemInfo item)
    {
        var json = ToNewJson(item);
        return ToItem(await Container.CreateItemAsync(json, new PartitionKey(json.Id)));
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await Container.ReadItemAsync<TJson>(id.ToString(), new PartitionKey(id.ToString()));
            return ToItem(response.Resource);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var query = Container.GetItemQueryIterator<TJson>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.Select(ToItem));
        }
        return results;
    }

    public async Task<T> DeleteAsync(Guid id)
    {
        return ToItem(await Container.DeleteItemAsync<TJson>(id.ToString(), new PartitionKey(id.ToString())));
    }

    protected Guid NewId()
    {
        return _idGenerator.NewId();
    }

    protected abstract TJson ToNewJson(TNewItemInfo item);
    protected abstract T ToItem(TJson json);
}
