namespace Claims.Domain;

public interface IClaimsService
{
    Task CreateClaimAsync(ICreateClaimRequest request);
    Task<IEnumerable<Claim>> GetAllClaimsAsync();
    Task DeleteClaimAsync(string id);
    Task<Claim?> GetClaimByIdAsync(string id);
}

public interface ICreateClaimRequest
{
    Guid CoverId { get; }
    string Name { get; }
    ClaimType Type { get; }
    decimal DamageCost { get; }
}