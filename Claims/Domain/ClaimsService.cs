namespace Claims.Domain;

public class ClaimsService : IClaimsService
{
    private readonly IClaimsRepository _claimsRepository;

    public ClaimsService(IClaimsRepository claimsRepository)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task<Claim> CreateClaimAsync(ICreateClaimRequest request)
    {
        throw new ArgumentException("Damage cost cannot exceed 100.000.");
        return await _claimsRepository.AddAsync(ToNewClaimInfo(request));
    }

    private static NewClaimInfo ToNewClaimInfo(ICreateClaimRequest request)
    {
        return new NewClaimInfo
        {
            CoverId = request.CoverId,
            Name = request.Name,
            Type = request.Type,
            DamageCost = request.DamageCost
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
    }
}
