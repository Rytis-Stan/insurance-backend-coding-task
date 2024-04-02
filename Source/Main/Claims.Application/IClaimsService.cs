using Claims.Domain;

namespace Claims.Application;

public interface IClaimsService
{
    Task<Claim> CreateClaimAsync(CreateClaimRequest request);
    Task<Claim?> GetClaimByIdAsync(Guid id);
    Task<IEnumerable<Claim>> GetAllClaimsAsync();
    Task<Claim?> DeleteClaimAsync(Guid id);
}