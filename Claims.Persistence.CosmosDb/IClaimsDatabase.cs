using Claims.Repositories;

namespace Claims.Persistence.CosmosDb;

public interface IClaimsDatabase
{
    IClaimsRepository ClaimsRepository { get; }
    ICoversRepository CoversRepository { get; }
    Task InitializeAsync();
}
