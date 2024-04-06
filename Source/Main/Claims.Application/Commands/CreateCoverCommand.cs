using BuildingBlocks.Temporal;
using Claims.Application.Repositories;
using Claims.Domain;

namespace Claims.Application.Commands;

public class CreateCoverCommand : ICreateCoverCommand
{
    private readonly ICoversRepository _coversRepository;
    private readonly ICoverPricing _coverPricing;
    private readonly IClock _clock;

    public CreateCoverCommand(ICoversRepository coversRepository, ICoverPricing coverPricing, IClock clock)
    {
        _coversRepository = coversRepository;
        _coverPricing = coverPricing;
        _clock = clock;
    }

    public async Task<Cover> ExecuteAsync(CreateCoverRequest request)
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
            throw new ValidationException("Start date cannot be in the past.");
        }
        if (endDate < utcToday)
        {
            throw new ValidationException("End date cannot be in the past.");
        }
        if (endDate < startDate)
        {
            throw new ValidationException("End date cannot be earlier than the start date.");
        }
        if (startDate.AddYears(1) <= endDate)
        {
            throw new ValidationException("Total insurance period cannot exceed 1 year.");
        }
    }

    private NewCoverInfo ToNewCoverInfo(CreateCoverRequest request)
    {
        return new NewCoverInfo(request.StartDate, request.EndDate, request.Type, PremiumFrom(request));
    }

    private decimal PremiumFrom(CreateCoverRequest request)
    {
        return _coverPricing.Premium(request.StartDate, request.EndDate, request.Type);
    }
}
