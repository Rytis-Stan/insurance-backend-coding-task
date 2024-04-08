namespace Claims.Api.Dto;

public record CreateClaimRequest(
    Guid CoverId,
    string Name,
    ClaimTypeDto Type,
    decimal DamageCost,
    DateTime Created
);
