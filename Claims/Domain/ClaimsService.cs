namespace Claims.Domain;

public class ClaimsService : IClaimsService
{
    private readonly IClaimsRepository _claimsRepository;
    private readonly ICoversRepository _coversRepository;

    public ClaimsService(IClaimsRepository claimsRepository, ICoversRepository coversRepository)
    {
        _claimsRepository = claimsRepository;
        _coversRepository = coversRepository;
    }

    public async Task<Claim> CreateClaimAsync(ICreateClaimRequest request)
    {
        await Validate(request);
        return await _claimsRepository.AddAsync(ToNewClaimInfo(request));
    }

    private async Task Validate(ICreateClaimRequest request)
    {
        var damageCost = request.DamageCost;
        if (damageCost == 0.00m)
        {
            throw new ArgumentException("Damage cost must be a positive value.");
        }
        if (damageCost > 100_000)
        {
            throw new ArgumentException("Damage cost cannot exceed 100.000.");
        }
        var cover = await _coversRepository.GetByIdAsync(request.CoverId);
        if (cover == null)
        {
            throw new ArgumentException("Claim references a non-existing cover via the cover ID.");
        }
        var created = DateOnly.FromDateTime(request.Created);
        if (created < cover.StartDate ||
            created > cover.EndDate)
        {
            throw new ArgumentException("Claim is outside the related cover period.");
        }
    }

    private static INewClaimInfo ToNewClaimInfo(ICreateClaimRequest request)
    {
        return new NewClaimInfo(request.CoverId, request.Name, request.Type, request.DamageCost, request.Created);
    }

    public Task<Claim> DeleteClaimAsync(Guid id)
    {
        return _claimsRepository.DeleteAsync(id);
    }

    public async Task<Claim?> GetClaimByIdAsync(Guid id)
    {
        return await _claimsRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        return await _claimsRepository.GetAllAsync();
    }
}