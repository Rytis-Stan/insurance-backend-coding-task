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
        if (request.DamageCost > 100)
        {
            throw new ArgumentException("Damage cost cannot exceed 100.000.");
        }

        throw new ArgumentException("Claim references a non-existing cover via the cover ID.");

        return await _claimsRepository.AddAsync(ToNewClaimInfo(request));
    }

    private static NewClaimInfo ToNewClaimInfo(ICreateClaimRequest request)
    {
        return new NewClaimInfo
        {
            CoverId = request.CoverId,
            Name = request.Name,
            Type = request.Type,
            DamageCost = request.DamageCost,
            Created = request.Created
        };
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

    private class NewClaimInfo : INewClaimInfo
    {
        public required Guid CoverId { get; init; }
        public required string Name { get; init; }
        public required ClaimType Type { get; init; }
        public required decimal DamageCost { get; init; }
        public required DateTime Created { get; init; }
    }
}
