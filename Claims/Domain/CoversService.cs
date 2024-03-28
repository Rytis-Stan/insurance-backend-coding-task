using Claims.Auditing;

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

    public async Task<Cover> CreateCoverAsync(ICreateCoverRequest request)
    {
        var coverToCreate = new Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Type = request.Type,
            Premium = Cover.ComputePremium(request.StartDate, request.EndDate, request.Type)
        };
        var cover = await _coversRepository.AddItemAsync(coverToCreate);
        _auditor.AuditCover(coverToCreate.Id, "POST");
        return cover;
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

    public async Task<Cover> DeleteCoverAsync(string id)
    {
        _auditor.AuditCover(id, "DELETE");
        return await _coversRepository.DeleteItemAsync(id);
    }
}