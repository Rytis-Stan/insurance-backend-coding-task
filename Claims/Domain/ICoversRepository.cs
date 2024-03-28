namespace Claims.Domain;

public interface ICoversRepository
{
    Task<Cover> AddAsync(INewCoverInfo item);
    Task<Cover?> GetByIdAsync(Guid id);
    Task<IEnumerable<Cover>> GetAllAsync();
    Task<Cover> DeleteAsync(Guid id);
}

public interface INewCoverInfo
{
    DateOnly StartDate { get; }
    DateOnly EndDate { get; }
    CoverType Type { get; }
    decimal Premium { get; }
}
