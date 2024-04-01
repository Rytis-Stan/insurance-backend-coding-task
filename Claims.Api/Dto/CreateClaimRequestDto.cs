using Claims.Domain;

namespace Claims.Dto;

public record CreateClaimRequestDto(
    Guid CoverId,
    string Name,
    ClaimType Type,
    decimal DamageCost,
    DateTime Created
) : ICreateClaimRequest;
