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
        return await _coversRepository.AddAsync(coverToCreate);
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
        return await _coversRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Cover>> GetAllCoversAsync()
    {
        return await _coversRepository.GetAllAsync();
    }

    public async Task<Cover> DeleteCoverAsync(Guid id)
    {
        return await _coversRepository.DeleteAsync(id);
    }

    private class NewCoverInfo : INewCoverInfo
    {
        public required DateOnly StartDate { get; init; }
        public required DateOnly EndDate { get; init; }
        public required CoverType Type { get; init; }
        public required decimal Premium { get; init; }
    }
}
