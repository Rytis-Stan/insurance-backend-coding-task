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
        var claimToCreate = new NewClaimInfo
        {
            CoverId = request.CoverId,
            Name = request.Name,
            Type = request.Type,
            DamageCost = request.DamageCost
        };
        return await _claimsRepository.AddAsync(claimToCreate);
    }

    public Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        return _claimsRepository.GetAllAsync();
    }

    public Task DeleteClaimAsync(Guid id)
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
