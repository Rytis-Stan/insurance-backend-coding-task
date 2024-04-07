using Claims.Domain;

namespace Claims.Application.Commands.CreateClaim;

public record CreateClaimRequest(
    Guid CoverId,
    string Name,
    ClaimType Type,
    decimal DamageCost,
    DateTime Created
);
