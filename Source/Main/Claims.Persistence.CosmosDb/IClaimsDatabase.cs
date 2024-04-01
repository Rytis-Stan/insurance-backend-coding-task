using Claims.Domain.Repositories;

namespace Claims.Persistence.CosmosDb;

public interface IClaimsDatabase
{
    IClaimsRepository ClaimsRepository { get; }
    ICoversRepository CoversRepository { get; }
    Task InitializeAsync();
}
