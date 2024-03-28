using Claims.Domain;
using Claims.Infrastructure;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public class CosmosDbClaimsRepository : CosmosDbRepository<Claim, INewClaimInfo, ClaimJson>, IClaimsRepository
{
    public CosmosDbClaimsRepository(CosmosClient dbClient, string databaseName, string containerName, IClock clock, IIdGenerator idGenerator)
        : base(dbClient, databaseName, containerName, clock, idGenerator)
    {
    }

    protected override ClaimJson ToNewJson(INewClaimInfo item, string id, DateTime created)
    {
        return new ClaimJson
        {
            Id = id,
            CoverId = item.CoverId.ToString(),
            Name = item.Name,
            Type = item.Type,
            DamageCost = item.DamageCost,
            Created = created
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