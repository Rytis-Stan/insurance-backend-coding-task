using Claims.Domain;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public class CosmosDbCoversRepository : CosmosDbRepository, ICoversRepository
{
    public CosmosDbCoversRepository(CosmosClient dbClient, string databaseName, string containerName)
        : base(dbClient, databaseName, containerName)
    {
    }

    public async Task<Cover?> GetCoverAsync(Guid id)
    {
        var cover = await GetByIdAsync<CoverJson>(id);
        return cover != null
            ? ToItem(cover)
            : null;
    }

    public async Task<IEnumerable<Cover>> GetAllCoversAsync()
    {
        return (await GetAllAsync<CoverJson>()).Select(ToItem);
    }

    public async Task<Cover> AddItemAsync(Cover item)
    {
        return ToItem(await Container.CreateItemAsync(ToJson(item), new PartitionKey(item.Id)));
    }

    public async Task<Cover> DeleteItemAsync(Guid id)
    {
        return ToItem(await Container.DeleteItemAsync<CoverJson>(id.ToString(), new PartitionKey(id.ToString())));
    }

    private Cover ToItem(CoverJson json)
    {
        return new Cover
        {
            Id = json.Id,
            StartDate = json.StartDate,
            EndDate = json.EndDate,
            Type = json.Type,
            Premium = json.Premium
        };
    }

    private CoverJson ToJson(Cover item)
    {
        return new CoverJson
        {
            Id = item.Id,
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            Type = item.Type,
            Premium = item.Premium
        };
    }
}
