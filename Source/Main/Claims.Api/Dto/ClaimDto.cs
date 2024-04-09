namespace Claims.Api.Dto;

public record ClaimDto(
    Guid Id,
    Guid CoverId,
    string Name,
    ClaimDtoType Type,
    decimal DamageCost,
    DateTime Created
);
