namespace Claims.Api.Dto;

public record ClaimDto(
    Guid Id,
    Guid CoverId,
    string Name,
    ClaimTypeDto Type,
    decimal DamageCost,
    DateTime Created
);
