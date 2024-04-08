using Claims.Domain;

namespace Claims.Application.Commands.CreateClaim;

public record CreateClaimArgs(
    Guid CoverId,
    string Name,
    ClaimType Type,
    decimal DamageCost,
    DateTime Created
);
