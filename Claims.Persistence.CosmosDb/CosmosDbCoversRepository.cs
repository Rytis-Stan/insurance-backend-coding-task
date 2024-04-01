using Claims.Infrastructure;
using Claims.Repositories;
using Microsoft.Azure.Cosmos;

namespace Claims.Persistence.CosmosDb;

public class CosmosDbCoversRepository : CosmosDbRepository<Cover, INewCoverInfo, CoverJson>, ICoversRepository
{
    public CosmosDbCoversRepository(CosmosClient dbClient, string databaseName, string containerName, IIdGenerator idGenerator)
        : base(dbClient, databaseName, containerName, idGenerator)
    {
    }

    protected override CoverJson ToNewJson(string id, INewCoverInfo item)
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
