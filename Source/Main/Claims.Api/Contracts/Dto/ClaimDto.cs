namespace Claims.Api.Contracts.Dto;

public record ClaimDto(
    Guid Id,
    Guid CoverId,
    string Name,
    ClaimDtoType Type,
    decimal DamageCost,
    DateTime Created
);
