namespace Claims.Domain;

public interface ICoversService
{
    Task<Cover> CreateCoverAsync(ICreateCoverRequest request);
    decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType);
    Task<Cover?> GetCoverAsync(Guid id);
    Task<IEnumerable<Cover>> GetAllCoversAsync();
    Task<Cover> DeleteCoverAsync(Guid id);
}
