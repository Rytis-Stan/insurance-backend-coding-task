namespace Claims.Domain;

public interface IClaimsService
{
    Task<Claim> CreateClaimAsync(ICreateClaimRequest request);
    Task<IEnumerable<Claim>> GetAllClaimsAsync();
    Task DeleteClaimAsync(Guid id);
    Task<Claim?> GetClaimByIdAsync(Guid id);
}

public interface ICreateClaimRequest
{
    Guid CoverId { get; }
    string Name { get; }
    ClaimType Type { get; }
    decimal DamageCost { get; }
}