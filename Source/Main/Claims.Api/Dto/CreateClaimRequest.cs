namespace Claims.Api.Dto;

public record CreateClaimRequest(
    Guid CoverId,
    string Name,
    ClaimDtoType Type,
    decimal DamageCost,
    DateTime Created
);
