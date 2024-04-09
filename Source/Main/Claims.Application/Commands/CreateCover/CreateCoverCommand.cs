using BuildingBlocks.Temporal;
using Claims.Application.Repositories;
using Claims.Domain;

namespace Claims.Application.Commands.CreateCover;

public class CreateCoverCommand : ICommand<CreateCoverArgs, CreateCoverResult>
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

    public async Task<CreateCoverResult> ExecuteAsync(CreateCoverArgs args)
    {
        Validate(args);
        var cover = await _coversRepository.CreateAsync(ToNewCoverInfo(args));
        return new CreateCoverResult(cover);
    }

    private void Validate(CreateCoverArgs args)
    {
        var utcToday = DateOnly.FromDateTime(_clock.UtcNow());
        var startDate = args.StartDate;
        var endDate = args.EndDate;

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
    }

    private NewCoverInfo ToNewCoverInfo(CreateCoverArgs args)
    {
        return new NewCoverInfo(args.StartDate, args.EndDate, args.Type, PremiumFrom(args));
    }

    private decimal PremiumFrom(CreateCoverArgs args)
    {
        return _coverPricing.Premium(args.StartDate, args.EndDate, args.Type);
    }
}
