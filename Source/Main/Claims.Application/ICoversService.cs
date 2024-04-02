using Claims.Domain;

namespace Claims.Application;

public interface ICoversService
{
    Task<Cover> CreateCoverAsync(CreateCoverRequest request);
    Task<Cover?> GetCoverAsync(Guid id);
    Task<IEnumerable<Cover>> GetAllCoversAsync();
    Task<Cover?> DeleteCoverAsync(Guid id);
}
