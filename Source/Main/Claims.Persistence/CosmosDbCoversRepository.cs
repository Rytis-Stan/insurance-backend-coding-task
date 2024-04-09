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

    protected override CoverItem ToItem(string id, NewCoverInfo entity)
    {
        return new CoverItem
        {
            Id = id,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Type = entity.Type,
            Premium = entity.Premium
        };
    }

    protected override Cover ToEntity(CoverItem item)
    {
        return new Cover
        {
            Id = Guid.Parse(item.Id),
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            Type = item.Type,
            Premium = item.Premium
        };
    }
}
