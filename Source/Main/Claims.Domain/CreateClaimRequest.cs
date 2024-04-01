namespace Claims.Domain;

public record CreateClaimRequest(
    Guid CoverId,
    string Name,
    ClaimType Type,
    decimal DamageCost,
    DateTime Created
);
