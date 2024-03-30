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
        if (request.DamageCost == 0.00m)
        {
            throw new ArgumentException("Damage cost must be a positive value.");
        }
        if (request.DamageCost > 100_000)
        {
            throw new ArgumentException("Damage cost cannot exceed 100.000.");
        }

        if (await _coversRepository.GetByIdAsync(request.CoverId) == null)
        {
            throw new ArgumentException("Claim references a non-existing cover via the cover ID.");
        }
        //
        // throw new ArgumentException("Claim references a non-existing cover via the cover ID.");

        // throw new ArgumentException("Claim is outside of related cover period.");

        return await _claimsRepository.AddAsync(ToNewClaimInfo(request));
    }

    private static INewClaimInfo ToNewClaimInfo(ICreateClaimRequest request)
    {
        return new NewClaimInfo(request.CoverId, request.Name, request.Type, request.DamageCost, request.Created);
    }

    public Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        return _claimsRepository.GetAllAsync();
    }

    public Task<Claim> DeleteClaimAsync(Guid id)
    {
        return _claimsRepository.DeleteAsync(id);
    }

    public Task<Claim?> GetClaimByIdAsync(Guid id)
    {
        return _claimsRepository.GetByIdAsync(id);
    }
}