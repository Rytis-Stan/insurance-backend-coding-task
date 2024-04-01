namespace Claims.Domain;

public record NewClaimInfo(
    Guid CoverId,
    string Name,
    ClaimType Type,
    decimal DamageCost,
    DateTime Created
) : INewClaimInfo;
