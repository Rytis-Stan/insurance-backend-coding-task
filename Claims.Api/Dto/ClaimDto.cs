namespace Claims.Api.Dto;

public record ClaimDto(
    Guid Id,
    Guid CoverId,
    DateTime Created,
    string Name,
    ClaimType Type,
    decimal DamageCost
);
