using Claims.Application.Repositories;
using Claims.Domain;

namespace Claims.Application;

public class ClaimsService : IClaimsService
{
    private readonly IClaimsRepository _claimsRepository;
    private readonly ICoversRepository _coversRepository;

    public ClaimsService(IClaimsRepository claimsRepository, ICoversRepository coversRepository)
    {
        _claimsRepository = claimsRepository;
        _coversRepository = coversRepository;
    }

    public async Task<Claim> CreateClaimAsync(CreateClaimRequest request)
    {
        await Validate(request);
        return await _claimsRepository.CreateAsync(ToNewClaimInfo(request));
    }

    private async Task Validate(CreateClaimRequest request)
    {
        var damageCost = request.DamageCost;
        if (damageCost <= 0.00m)
        {
            throw new ArgumentException("Damage cost must be a positive value.");
        }
        const decimal maxAllowedDamageCost = 100_000;
        if (damageCost > maxAllowedDamageCost)
        {
            throw new ArgumentException($"Damage cost cannot exceed {maxAllowedDamageCost}.");
        }
        var cover = await _coversRepository.FindByIdAsync(request.CoverId);
#pragma warning disable IDE0270 // Use coalesce expression
        if (cover == null)
        {
            throw new ArgumentException("Claim references a non-existing cover via the cover ID.");
        }
#pragma warning restore IDE0270 // Use coalesce expression
        var created = DateOnly.FromDateTime(request.Created);
        if (created < cover.StartDate ||
            created > cover.EndDate)
        {
            throw new ArgumentException("Claim is outside the related cover period.");
        }
    }

    private static NewClaimInfo ToNewClaimInfo(CreateClaimRequest request)
    {
        return new NewClaimInfo(request.CoverId, request.Name, request.Type, request.DamageCost, request.Created);
    }

    public async Task<Claim?> GetClaimByIdAsync(Guid id)
    {
        return await _claimsRepository.FindByIdAsync(id);
    }

    public async Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        return await _claimsRepository.GetAllAsync();
    }

    public async Task<Claim?> DeleteClaimAsync(Guid id)
    {
        return await _claimsRepository.DeleteByIdAsync(id);
    }
}