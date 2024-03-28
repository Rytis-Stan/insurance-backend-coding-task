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
        return await _claimsRepository.AddItemAsync(claimToCreate);
    }

    public Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        return _claimsRepository.GetAllClaimsAsync();
    }

    public Task DeleteClaimAsync(Guid id)
    {
        return _claimsRepository.DeleteItemAsync(id);
    }

    public Task<Claim?> GetClaimByIdAsync(Guid id)
    {
        return _claimsRepository.GetClaimAsync(id);
    }
}

public class NewClaimInfo : INewClaimInfo
{
    public Guid CoverId { get; init; }
    public string Name { get; init; }
    public ClaimType Type { get; init; }
    public decimal DamageCost { get; init; }
}