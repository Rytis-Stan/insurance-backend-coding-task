using Microsoft.Azure.Cosmos;

namespace Claims.DataAccess;

public abstract class CosmosDbRepository
{
    protected readonly Container Container;

    protected CosmosDbRepository(CosmosClient dbClient, string databaseName, string containerName)
    {
        ArgumentNullException.ThrowIfNull(dbClient, nameof(dbClient));
        Container = dbClient.GetContainer(databaseName, containerName);
    }
}
