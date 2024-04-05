using Claims.Application.Repositories;
using Claims.Domain;
using Microsoft.Azure.Cosmos;

namespace Claims.Persistence.Claims;

internal class CosmosDbCoversRepository : CosmosDbRepository<NewCoverInfo, Cover, CoverJson>, ICoversRepository
{
    public CosmosDbCoversRepository(CosmosClient dbClient, string databaseName, IIdSource idSource)
        : base(dbClient, databaseName, ContainerNames.Cover, idSource)
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