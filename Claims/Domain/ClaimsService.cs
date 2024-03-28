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
        var claimToCreate = new Claim
        {
            Id = Guid.NewGuid().ToString(), // TODO: Change the domain object's "Id" to an actual GUID value
            CoverId = request.CoverId.ToString(), // TODO: Change the domain object's "CoverId" to an actual GUID value
            Created = DateTime.Now, // TODO: Move "Created" field initialization into the repository
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