using Claims.Application.Repositories;
using Claims.Domain;
using Claims.Persistence.Items;
using Microsoft.Azure.Cosmos;

namespace Claims.Persistence;

internal class CosmosDbClaimsRepository : CosmosDbRepository<NewClaimInfo, Claim, ClaimItem>, IClaimsRepository
{
    public CosmosDbClaimsRepository(CosmosClient dbClient, string databaseName, IIdSource idSource)
        : base(dbClient, databaseName, ContainerNames.Claim, idSource)
    {
    }

    protected override ClaimItem ToItem(string id, NewClaimInfo entity)
    {
        return new ClaimItem
        {
            Id = id,
            CoverId = entity.CoverId.ToString(),
            Name = entity.Name,
            Type = entity.Type,
            DamageCost = entity.DamageCost,
            Created = entity.Created
        };
    }

    protected override Claim ToEntity(ClaimItem item)
    {
        return new Claim
        {
            Id = Guid.Parse(item.Id),
            CoverId = Guid.Parse(item.CoverId),
            Name = item.Name,
            Type = item.Type,
            DamageCost = item.DamageCost,
            Created = item.Created
        };
    }
}