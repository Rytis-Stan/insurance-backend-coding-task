namespace Claims.Domain;

public interface ICoversRepository
{
    Task<Cover> AddItemAsync(Cover item);
    Task<Cover?> GetCoverAsync(string id);
    Task<IEnumerable<Cover>> GetAllCoversAsync();
    Task<Cover> DeleteItemAsync(string id);
}
