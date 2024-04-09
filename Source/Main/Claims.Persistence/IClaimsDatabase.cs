using Claims.Application.Repositories;

namespace Claims.Persistence;

public interface IClaimsDatabase
{
    IClaimsRepository ClaimsRepository { get; }
    ICoversRepository CoversRepository { get; }
    Task InitializeAsync();
}
