namespace Claims.Domain;

public interface ICoversService
{
    Task<Cover> CreateCoverAsync(CreateCoverRequest request);
    Task<Cover?> GetCoverAsync(Guid id);
    Task<IEnumerable<Cover>> GetAllCoversAsync();
    Task<Cover?> DeleteCoverAsync(Guid id);
}
