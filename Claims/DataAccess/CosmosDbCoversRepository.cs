using Claims.Domain;
using Claims.Infrastructure;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public class CosmosDbCoversRepository : CosmosDbRepository, ICoversRepository
{
    public CosmosDbCoversRepository(CosmosClient dbClient, string databaseName, string containerName, IClock clock)
        : base(dbClient, databaseName, containerName, clock)
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

    public async Task<Cover> AddItemAsync(INewCoverInfo item)
    {
        var json = ToNewJson(item);
        return ToItem(await Container.CreateItemAsync(json, new PartitionKey(json.Id)));
    }

    private CoverJson ToNewJson(INewCoverInfo item)
    {
        return new CoverJson
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            Type = item.Type,
            Premium = item.Premium,
            Created = Clock.Now()
        };
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
            Premium = json.Premium,
            Created = json.Created
        };
    }
}
