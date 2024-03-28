namespace Claims.Domain;

public interface ICoversRepository
{
    Task<Cover> AddItemAsync(Cover item);
    Task<Cover?> GetCoverAsync(Guid id);
    Task<IEnumerable<Cover>> GetAllCoversAsync();
    Task<Cover> DeleteItemAsync(Guid id);
}
