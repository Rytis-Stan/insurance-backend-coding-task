using Claims.Domain;
using Claims.Infrastructure;
using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public class CosmosDbCoversRepository : CosmosDbRepository<Cover, INewCoverInfo, CoverJson>, ICoversRepository
{
    public CosmosDbCoversRepository(CosmosClient dbClient, string databaseName, string containerName, IClock clock, IIdGenerator idGenerator)
        : base(dbClient, databaseName, containerName, clock, idGenerator)
    {
    }

    protected override CoverJson ToNewJson(INewCoverInfo item)
    {
        return new CoverJson
        {
            Id = NewId().ToString(),
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            Type = item.Type,
            Premium = item.Premium,
            Created = Clock.UtcNow()
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
            Premium = json.Premium,
            Created = json.Created
        };
    }
}
