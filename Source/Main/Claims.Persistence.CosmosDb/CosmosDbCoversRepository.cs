using Claims.Domain;
using Claims.Domain.Repositories;
using Claims.Infrastructure;
using Microsoft.Azure.Cosmos;

namespace Claims.Persistence.CosmosDb;

internal class CosmosDbCoversRepository : CosmosDbRepository<Cover, NewCoverInfo, CoverJson>, ICoversRepository
{
    public CosmosDbCoversRepository(CosmosClient dbClient, string databaseName, IIdGenerator idGenerator)
        : base(dbClient, databaseName, ContainerNames.Cover, idGenerator)
    {
    }

    protected override CoverJson ToNewJson(string id, NewCoverInfo item)
    {
        return new CoverJson
        {
            Id = id,
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            Type = item.Type,
            Premium = item.Premium
        };
    }

    protected override Cover ToItem(CoverJson json)
    {
        return new Cover
        {
            Id = Guid.Parse(json.Id),
            StartDate = json.StartDate,
            EndDate = json.EndDate,
            Type = json.Type,
            Premium = json.Premium
        };
    }
}
