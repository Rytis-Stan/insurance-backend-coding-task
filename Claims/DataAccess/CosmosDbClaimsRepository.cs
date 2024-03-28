using Claims.Domain;
using Claims.Infrastructure;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public class CosmosDbClaimsRepository : CosmosDbRepository, IClaimsRepository
{
    public CosmosDbClaimsRepository(CosmosClient dbClient, string databaseName, string containerName, IClock clock, IIdGenerator idGenerator)
        : base(dbClient, databaseName, containerName, clock, idGenerator)
    {
    }

    public async Task<Claim?> GetByIdAsync(Guid id)
    {
        var json = await GetByIdAsync<ClaimJson>(id);
        return json != null
            ? ToItem(json)
            : null;
    }

    public async Task<IEnumerable<Claim>> GetAllAsync()
    {
        return (await GetAllAsync<ClaimJson>()).Select(ToItem);
    }

    public async Task<Claim> AddAsync(INewClaimInfo item)
    {
        var json = ToNewJson(item);
        return ToItem(await Container.CreateItemAsync(json, new PartitionKey(json.Id)));
    }

    private ClaimJson ToNewJson(INewClaimInfo item)
    {
        return new ClaimJson
        {
            Id = NewId().ToString(),
            CoverId = item.CoverId.ToString(),
            Name = item.Name,
            Type = item.Type,
            DamageCost = item.DamageCost,
            Created = Clock.UtcNow()
        };
    }

    public async Task<Claim> DeleteAsync(Guid id)
    {
        return ToItem(await Container.DeleteItemAsync<ClaimJson>(id.ToString(), new PartitionKey(id.ToString())));
    }

    private Claim ToItem(ClaimJson json)
    {
        return new Claim
        {
            Id = Guid.Parse(json.Id),
            CoverId = Guid.Parse(json.CoverId),
            Name = json.Name,
            Type = json.Type,
            DamageCost = json.DamageCost,
            Created = json.Created
        };
    }
}