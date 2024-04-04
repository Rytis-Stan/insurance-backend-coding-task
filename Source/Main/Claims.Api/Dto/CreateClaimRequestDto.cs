namespace Claims.Api.Dto;

public record CreateClaimRequestDto(
    Guid CoverId,
    string Name,
    ClaimTypeDto Type,
    decimal DamageCost,
    DateTime Created
);