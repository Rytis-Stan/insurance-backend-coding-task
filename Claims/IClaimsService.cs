namespace Claims;

public interface IClaimsService
{
    Task<Claim> CreateClaimAsync(ICreateClaimRequest request);
    Task<Claim?> GetClaimByIdAsync(Guid id);
    Task<IEnumerable<Claim>> GetAllClaimsAsync();
    Task<Claim?> DeleteClaimAsync(Guid id);
}

public interface ICreateClaimRequest
{
    Guid CoverId { get; }
    string Name { get; }
    ClaimType Type { get; }
    decimal DamageCost { get; }
    DateTime Created { get; }
}

public record CreateClaimRequest(
    Guid CoverId,
    string Name,
    ClaimType Type,
    decimal DamageCost,
    DateTime Created
) : ICreateClaimRequest;