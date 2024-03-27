using Microsoft.Azure.Cosmos;

namespace Claims.Domain;

public interface ICoversRepository
{
    Task<ItemResponse<Cover>> AddItemAsync(Cover cover);
    Task<Cover?> GetCoverAsync(string id);
    Task<IEnumerable<Cover>> GetAllCoversAsync();
    Task<ItemResponse<Cover>> DeleteCoverAsync(string id);
}
