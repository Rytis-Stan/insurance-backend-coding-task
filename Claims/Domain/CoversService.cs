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
        var coverToCreate = ToNewCoverInfo(request);
        return await _coversRepository.AddItemAsync(coverToCreate);
    }

    private static INewCoverInfo ToNewCoverInfo(ICreateCoverRequest request)
    {
        return new NewCoverInfo
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Type = request.Type,
            Premium = Cover.ComputePremium(request.StartDate, request.EndDate, request.Type)
        };
    }

    public decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return Cover.ComputePremium(startDate, endDate, coverType);
    }

    public async Task<Cover?> GetCoverAsync(Guid id)
    {
        return await _coversRepository.GetCoverAsync(id);
    }

    public async Task<IEnumerable<Cover>> GetAllCoversAsync()
    {
        return await _coversRepository.GetAllCoversAsync();
    }

    public async Task<Cover> DeleteCoverAsync(Guid id)
    {
        return await _coversRepository.DeleteItemAsync(id);
    }

    private class NewCoverInfo : INewCoverInfo
    {
        public DateOnly StartDate { get; init; }
        public DateOnly EndDate { get; init; }
        public CoverType Type { get; init; }
        public decimal Premium { get; init; }
    }
}
