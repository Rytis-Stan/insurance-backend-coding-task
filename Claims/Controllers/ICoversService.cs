using Microsoft.Azure.Cosmos;

namespace Claims.Controllers;

public interface ICoversService
{
    Task CreateCoverAsync(Cover cover);
    decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType);
    Task<Cover?> GetCoverAsync(string id);
    Task<List<Cover>> GetAllCoversAsync();
    Task<ItemResponse<Cover>> DeleteCoverAsync(string id);
}
