namespace Claims.Domain;

public interface IClaimsService
{
    Task CreateClaimAsync(Claim claim);
    Task<IEnumerable<Claim>> GetAllClaimsAsync();
    Task DeleteClaimAsync(string id);
    Task<Claim> GetClaimByIdAsync(string id);
}