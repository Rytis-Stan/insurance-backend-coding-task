using Claims.Application.Repositories;
using Claims.Domain;
using Claims.Persistence.Items;
using Microsoft.Azure.Cosmos;

namespace Claims.Persistence;

internal class CosmosDbCoversRepository : CosmosDbRepository<NewCoverInfo, Cover, CoverItem>, ICoversRepository
{
    public CosmosDbCoversRepository(CosmosClient dbClient, string databaseName, IIdSource idSource)
        : base(dbClient, databaseName, ContainerNames.Cover, idSource)
    {
    }

    protected override CoverItem ToItem(string id, NewCoverInfo entityInfo)
    {
        return new CoverItem
        {
            Id = id,
            StartDate = entityInfo.StartDate,
            EndDate = entityInfo.EndDate,
            Type = ToDbEnum(entityInfo.Type),
            Premium = entityInfo.Premium
        };
    }

    protected override Cover ToEntity(CoverItem item)
    {
        return new Cover
        {
            Id = Guid.Parse(item.Id),
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            Type = ToDomainEnum(item.Type),
            Premium = item.Premium
        };
    }

    private static CoverItemType ToDbEnum(CoverType source)
    {
        return source switch
        {
            CoverType.Yacht => CoverItemType.Yacht,
            CoverType.PassengerShip => CoverItemType.PassengerShip,
            CoverType.ContainerShip => CoverItemType.ContainerShip,
            CoverType.BulkCarrier => CoverItemType.BulkCarrier,
            CoverType.Tanker => CoverItemType.Tanker,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }

    private static CoverType ToDomainEnum(CoverItemType source)
    {
        return source switch
        {
            CoverItemType.Yacht => CoverType.Yacht,
            CoverItemType.PassengerShip => CoverType.PassengerShip,
            CoverItemType.ContainerShip => CoverType.ContainerShip,
            CoverItemType.BulkCarrier => CoverType.BulkCarrier,
            CoverItemType.Tanker => CoverType.Tanker,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }
}
