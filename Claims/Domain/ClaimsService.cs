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

    public async Task CreateClaimAsync(Claim claim)
    {
        claim.Id = Guid.NewGuid().ToString();
        await _claimsRepository.AddItemAsync(claim);
        _auditor.AuditClaim(claim.Id, "POST");
    }

    public Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        return _claimsRepository.GetClaimsAsync();
    }

    public Task DeleteClaimAsync(string id)
    {
        _auditor.AuditClaim(id, "DELETE");
        var deletedClaim = _claimsRepository.DeleteItemAsync(id);
        return deletedClaim;
    }

    public Task<Claim> GetClaimByIdAsync(string id)
    {
        return _claimsRepository.GetClaimAsync(id);
    }
}