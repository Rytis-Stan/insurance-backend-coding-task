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

    protected override ClaimItem ToItem(string id, NewClaimInfo entityInfo)
    {
        return new ClaimItem
        {
            Id = id,
            CoverId = entityInfo.CoverId.ToString(),
            Name = entityInfo.Name,
            Type = ToDbEnum(entityInfo.Type),
            DamageCost = entityInfo.DamageCost,
            Created = entityInfo.Created
        };
    }

    protected override Claim ToEntity(ClaimItem item)
    {
        return new Claim
        {
            Id = Guid.Parse(item.Id),
            CoverId = Guid.Parse(item.CoverId),
            Name = item.Name,
            Type = ToDomainEnum(item.Type),
            DamageCost = item.DamageCost,
            Created = item.Created
        };
    }

    private static ClaimItemType ToDbEnum(ClaimType source)
    {
        return source switch
        {
            ClaimType.Collision => ClaimItemType.Collision,
            ClaimType.Grounding => ClaimItemType.Grounding,
            ClaimType.BadWeather => ClaimItemType.BadWeather,
            ClaimType.Fire => ClaimItemType.Fire,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }

    private static ClaimType ToDomainEnum(ClaimItemType source)
    {
        return source switch
        {
            ClaimItemType.Collision => ClaimType.Collision,
            ClaimItemType.Grounding => ClaimType.Grounding,
            ClaimItemType.BadWeather => ClaimType.BadWeather,
            ClaimItemType.Fire => ClaimType.Fire,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }
}