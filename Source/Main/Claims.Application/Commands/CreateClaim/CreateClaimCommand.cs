using Claims.Application.Repositories;

namespace Claims.Application.Commands.CreateClaim;

public class CreateClaimCommand : ICommand<CreateClaimRequest, CreateClaimResponse>
{
    private readonly IClaimsRepository _claimsRepository;
    private readonly ICoversRepository _coversRepository;

    public CreateClaimCommand(IClaimsRepository claimsRepository, ICoversRepository coversRepository)
    {
        _claimsRepository = claimsRepository;
        _coversRepository = coversRepository;
    }

    public async Task<CreateClaimResponse> ExecuteAsync(CreateClaimRequest request)
    {
        await Validate(request);
        var claim = await _claimsRepository.CreateAsync(ToNewClaimInfo(request));
        return new CreateClaimResponse(claim);
    }

    private async Task Validate(CreateClaimRequest request)
    {
        var damageCost = request.DamageCost;
        if (damageCost <= 0.00m)
        {
            throw new ValidationException("Damage cost must be a positive value.");
        }
        const decimal maxAllowedDamageCost = 100_000;
        if (damageCost > maxAllowedDamageCost)
        {
            throw new ValidationException($"Damage cost cannot exceed {maxAllowedDamageCost}.");
        }
        var cover = await _coversRepository.FindByIdAsync(request.CoverId);
#pragma warning disable IDE0270 // Use coalesce expression
        if (cover == null)
        {
            throw new ValidationException("Claim references a non-existing cover via the cover ID.");
        }
#pragma warning restore IDE0270 // Use coalesce expression
        var created = DateOnly.FromDateTime(request.Created);
        if (created < cover.StartDate ||
            created > cover.EndDate)
        {
            throw new ValidationException("Claim is outside the related cover period.");
        }
    }

    private static NewClaimInfo ToNewClaimInfo(CreateClaimRequest request)
    {
        return new NewClaimInfo(request.CoverId, request.Name, request.Type, request.DamageCost, request.Created);
    }
}
