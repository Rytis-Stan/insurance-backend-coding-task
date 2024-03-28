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

    public async Task<Claim> AddItemAsync(Claim item)
    {
        return ToItem(await Container.CreateItemAsync(ToJson(item), new PartitionKey(item.Id)));
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