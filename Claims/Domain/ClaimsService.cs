using Claims.Auditing;

namespace Claims.Domain;

public class ClaimsService : IClaimsService
{
    private readonly IClaimsRepository _claimsRepository;
    private readonly IClaimAuditor _auditor;

    public ClaimsService(IClaimsRepository claimsRepository, IClaimAuditor auditor)
    {
        _claimsRepository = claimsRepository;
        _auditor = auditor;
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
        var claim = await _claimsRepository.AddItemAsync(claimToCreate);
        _auditor.AuditClaim(claimToCreate.Id, "POST");
        return claim;
    }

    public Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        return _claimsRepository.GetAllClaimsAsync();
    }

    public Task DeleteClaimAsync(string id)
    {
        _auditor.AuditClaim(id, "DELETE");
        var deletedClaim = _claimsRepository.DeleteItemAsync(id);
        return deletedClaim;
    }

    public Task<Claim?> GetClaimByIdAsync(string id)
    {
        return _claimsRepository.GetClaimAsync(id);
    }
}