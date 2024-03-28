namespace Claims.Domain;

public class CoversService : ICoversService
{
    private readonly ICoversRepository _coversRepository;

    public CoversService(ICoversRepository coversRepository)
    {
        _coversRepository = coversRepository;
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
        return await _coversRepository.AddItemAsync(coverToCreate);
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
        return await _coversRepository.DeleteItemAsync(id);
    }
}