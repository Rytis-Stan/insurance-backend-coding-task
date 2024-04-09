using System.Net;
using Microsoft.Azure.Cosmos;

namespace Claims.Persistence;

public abstract class CosmosDbRepository<TNewDomainEntityInfo, TDomainEntity, TDbItem>
    where TNewDomainEntityInfo : class
    where TDomainEntity : class
    where TDbItem : class
{
    private readonly Container _container;
    private readonly IIdSource _idSource;

    protected CosmosDbRepository(CosmosClient dbClient, string databaseName, string containerName, IIdSource idSource)
    {
        ArgumentNullException.ThrowIfNull(dbClient, nameof(dbClient));
        _container = dbClient.GetContainer(databaseName, containerName);
        _idSource = idSource;
    }

    public async Task<TDomainEntity> CreateAsync(TNewDomainEntityInfo entity)
    {
        var id = _idSource.NewId().ToString();
        var item = ToItem(id, entity);
        return ToEntity(await _container.CreateItemAsync(item, new PartitionKey(id)));
    }

    public async Task<TDomainEntity?> FindByIdAsync(Guid id)
    {
        try
        {
            var response = await _container.ReadItemAsync<TDbItem>(id.ToString(), new PartitionKey(id.ToString()));
            return ToEntity(response.Resource);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<TDomainEntity>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<TDbItem>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<TDomainEntity>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.Select(ToEntity));
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
            await _container.DeleteItemAsync<TDbItem>(id, new PartitionKey(id));
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Item not found in the database. Let's assume that it's ok to do a delete
            // operation for a non-existing item. No further processing needed.
        }
    }

    protected abstract TDbItem ToItem(string id, TNewDomainEntityInfo entity);
    protected abstract TDomainEntity ToEntity(TDbItem item);
}
