using Claims.Infrastructure;
using Claims.Repositories;
using Microsoft.Azure.Cosmos;

namespace Claims.Persistence.CosmosDb;

public class CosmosDbClaimsRepository : CosmosDbRepository<Claim, INewClaimInfo, ClaimJson>, IClaimsRepository
{
    public CosmosDbClaimsRepository(CosmosClient dbClient, string databaseName, IIdGenerator idGenerator)
        : base(dbClient, databaseName, ContainerNames.Claim, idGenerator)
    {
    }

    protected override ClaimJson ToNewJson(string id, INewClaimInfo item)
    {
        return new ClaimJson
        {
            Id = id,
            CoverId = item.CoverId.ToString(),
            Name = item.Name,
            Type = item.Type,
            DamageCost = item.DamageCost,
            Created = item.Created
        };
    }

    protected override Claim ToItem(ClaimJson json)
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