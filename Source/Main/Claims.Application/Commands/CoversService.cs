using BuildingBlocks.Temporal;
using Claims.Application.Repositories;
using Claims.Domain;

namespace Claims.Application.Commands;

public class CoversService : ICreateCoverCommand, IGetCoverCommand
{
    private readonly ICoversRepository _coversRepository;
    private readonly ICoverPricing _coverPricing;
    private readonly IClock _clock;

    public CoversService(ICoversRepository coversRepository, ICoverPricing coverPricing, IClock clock)
    {
        _coversRepository = coversRepository;
        _coverPricing = coverPricing;
        _clock = clock;
    }

    public async Task<Cover> CreateCoverAsync(CreateCoverRequest request)
    {
        Validate(request);
        return await _coversRepository.CreateAsync(ToNewCoverInfo(request));
    }

    private void Validate(CreateCoverRequest request)
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

    private NewCoverInfo ToNewCoverInfo(CreateCoverRequest request)
    {
        return new NewCoverInfo(request.StartDate, request.EndDate, request.Type, PremiumFrom(request));
    }

    private decimal PremiumFrom(CreateCoverRequest request)
    {
        return _coverPricing.CalculatePremium(request.StartDate, request.EndDate, request.Type);
    }

    public async Task<Cover?> GetCoverAsync(Guid id)
    {
        return await _coversRepository.FindByIdAsync(id);
    }
}

public class GetAllCoversCommand : IGetAllCoversCommand
{
    private readonly ICoversRepository _coversRepository;

    public GetAllCoversCommand(ICoversRepository coversRepository)
    {
        _coversRepository = coversRepository;
    }

    public async Task<IEnumerable<Cover>> GetAllCoversAsync()
    {
        return await _coversRepository.GetAllAsync();
    }
}

public class DeleteCoverCommand : IDeleteCoverCommand
{
    private readonly ICoversRepository _coversRepository;

    public DeleteCoverCommand(ICoversRepository coversRepository)
    {
        _coversRepository = coversRepository;
    }

    public async Task<Cover?> DeleteCoverAsync(Guid id)
    {
        return await _coversRepository.DeleteByIdAsync(id);
    }
}