using Claims.Application.Repositories;

namespace Claims.Application.Commands.CreateClaim;

public class CreateClaimCommand : ICommand<CreateClaimArgs, CreateClaimResult>
{
    private const decimal MaxAllowedDamageCost = 100_000;

    private readonly IClaimsRepository _claimsRepository;
    private readonly ICoversRepository _coversRepository;

    public CreateClaimCommand(IClaimsRepository claimsRepository, ICoversRepository coversRepository)
    {
        _claimsRepository = claimsRepository;
        _coversRepository = coversRepository;
    }

    public async Task<CreateClaimResult> ExecuteAsync(CreateClaimArgs args)
    {
        await Validate(args);
        var claim = await _claimsRepository.CreateAsync(ToNewClaimInfo(args));
        return new CreateClaimResult(claim);
    }

    private async Task Validate(CreateClaimArgs args)
    {
        if (args.DamageCost <= 0.00m)
        {
            throw new ValidationException("Damage cost must be a positive value.");
        }
        if (args.DamageCost > MaxAllowedDamageCost)
        {
            throw new ValidationException($"Damage cost cannot exceed {MaxAllowedDamageCost}.");
        }

        var cover = await _coversRepository.FindByIdAsync(args.CoverId)
                    ?? throw new ValidationException("Claim references a non-existing cover via the cover ID.");

        var created = DateOnly.FromDateTime(args.Created);
        if (created < cover.StartDate ||
            created > cover.EndDate)
        {
            throw new ValidationException("Claim is outside the related cover period.");
        }
    }

    private static NewClaimInfo ToNewClaimInfo(CreateClaimArgs args)
    {
        return new NewClaimInfo(args.CoverId, args.Name, args.Type, args.DamageCost, args.Created);
    }
}
