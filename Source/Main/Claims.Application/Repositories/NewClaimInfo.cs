using Claims.Domain;

namespace Claims.Application.Repositories;

public record NewClaimInfo(
    Guid CoverId,
    string Name,
    ClaimType Type,
    decimal DamageCost,
    DateTime Created
);
