using Claims.Auditing;
using Microsoft.Azure.Cosmos;

namespace Claims.Domain;

public class CoversService : ICoversService
{
    private readonly ICoversRepository _coversRepository;
    private readonly ICoverAuditor _auditor;

    public CoversService(ICoversRepository coversRepository, ICoverAuditor auditor)
    {
        _coversRepository = coversRepository;
        _auditor = auditor;
    }

    public async Task CreateCoverAsync(ICreateCoverRequest request)
    {
        var cover = new Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Type = request.Type,
            Premium = Cover.ComputePremium(request.StartDate, request.EndDate, request.Type)
        };
        await _coversRepository.AddItemAsync(cover);
        _auditor.AuditCover(cover.Id, "POST");
    }

    public decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return Cover.ComputePremium(startDate, endDate, coverType);
    }

    public async Task<Cover?> GetCoverAsync(string id)
    {
        return await _coversRepository.GetCoverAsync(id);
    }

    public async Task<IEnumerable<Cover>> GetAllCoversAsync()
    {
        return await _coversRepository.GetAllCoversAsync();
    }

    public async Task<ItemResponse<Cover>> DeleteCoverAsync(string id)
    {
        _auditor.AuditCover(id, "DELETE");
        return await _coversRepository.DeleteItemAsync(id);
    }
}