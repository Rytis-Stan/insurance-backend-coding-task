using Claims.Infrastructure;
using Claims.Repositories;

namespace Claims;

public class CoversService : ICoversService
{
    private readonly ICoversRepository _coversRepository;
    private readonly IPricingService _pricingService;
    private readonly IClock _clock;

    public CoversService(ICoversRepository coversRepository, IPricingService pricingService, IClock clock)
    {
        _coversRepository = coversRepository;
        _pricingService = pricingService;
        _clock = clock;
    }

    public async Task<Cover> CreateCoverAsync(ICreateCoverRequest request)
    {
        Validate(request);
        return await _coversRepository.AddAsync(ToNewCoverInfo(request));
    }

    private void Validate(ICreateCoverRequest request)
    {
        var utcToday = DateOnly.FromDateTime(_clock.UtcNow());
        var startDate = request.StartDate;
        var endDate = request.EndDate;

        if (startDate < utcToday)
        {
            throw new ArgumentException("Start date cannot be in the past.");
        }
        if (endDate < utcToday)
        {
            throw new ArgumentException("End date cannot be in the past.");
        }
        if (endDate < startDate)
        {
            throw new ArgumentException("End date cannot be earlier than the start date.");
        }
        if (startDate.AddYears(1) <= endDate)
        {
            throw new ArgumentException("Total insurance period cannot exceed 1 year.");
        }
    }

    private INewCoverInfo ToNewCoverInfo(ICreateCoverRequest request)
    {
        return new NewCoverInfo(request.StartDate, request.EndDate, request.Type, PremiumFrom(request));
    }

    private decimal PremiumFrom(ICreateCoverRequest request)
    {
        return _pricingService.CalculatePremium(request.StartDate, request.EndDate, request.Type);
    }

    public async Task<Cover?> GetCoverAsync(Guid id)
    {
        return await _coversRepository.FindByIdAsync(id);
    }

    public async Task<IEnumerable<Cover>> GetAllCoversAsync()
    {
        return await _coversRepository.GetAllAsync();
    }

    public async Task<Cover?> DeleteCoverAsync(Guid id)
    {
        return await _coversRepository.DeleteAsync(id);
    }
}
