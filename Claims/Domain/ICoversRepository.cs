namespace Claims.Domain;

public interface ICoversRepository
{
    Task<Cover> AddItemAsync(INewCoverInfo item);
    Task<Cover?> GetCoverAsync(Guid id);
    Task<IEnumerable<Cover>> GetAllCoversAsync();
    Task<Cover> DeleteItemAsync(Guid id);
}

public interface INewCoverInfo
{
    DateOnly StartDate { get; }
    DateOnly EndDate { get; }
    CoverType Type { get; }
    decimal Premium { get; }
}
