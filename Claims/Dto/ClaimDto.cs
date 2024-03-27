using Claims.Domain;

namespace Claims.Dto;

public record ClaimDto(
    string Id,
    string CoverId,
    DateTime Created,
    string Name,
    ClaimType Type,
    decimal DamageCost
);
