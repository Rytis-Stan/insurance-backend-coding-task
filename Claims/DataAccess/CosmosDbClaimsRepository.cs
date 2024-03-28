using Claims.Domain;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public class CosmosDbClaimsRepository : CosmosDbRepository, IClaimsRepository
{
    public CosmosDbClaimsRepository(CosmosClient dbClient, string databaseName, string containerName)
        : base(dbClient, databaseName, containerName)
    {
    }

    public async Task<Claim?> GetClaimAsync(Guid id)
    {
        var json = await GetByIdAsync<ClaimJson>(id);
        return json != null
            ? ToItem(json)
            : null;
    }

    public async Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        return (await GetAllAsync<ClaimJson>()).Select(ToItem);
    }

    public async Task<Claim> AddItemAsync(INewClaimInfo item)
    {
        var json = ToNewJson(item);
        return ToItem(await Container.CreateItemAsync(json, new PartitionKey(json.Id)));
    }

    private ClaimJson ToNewJson(INewClaimInfo item)
    {
        return new ClaimJson
        {
            Id = Guid.NewGuid().ToString(),
            CoverId = item.CoverId.ToString(),
            Created = item.Created, // TODO: Inject current moment value here!!!
            Name = item.Name,
            Type = item.Type,
            DamageCost = item.DamageCost
        };
    }

    public async Task<Claim> DeleteItemAsync(Guid id)
    {
        return ToItem(await Container.DeleteItemAsync<ClaimJson>(id.ToString(), new PartitionKey(id.ToString())));
    }

    private Claim ToItem(ClaimJson json)
    {
        return new Claim
        {
            Id = json.Id,
            CoverId = json.CoverId,
            Created = json.Created,
            Name = json.Name,
            Type = json.Type,
            DamageCost = json.DamageCost
        };
    }

    private ClaimJson ToJson(Claim item)
    {
        return new ClaimJson
        {
            Id = item.Id,
            CoverId = item.CoverId,
            Created = item.Created,
            Name = item.Name,
            Type = item.Type,
            DamageCost = item.DamageCost
        };
    }
}