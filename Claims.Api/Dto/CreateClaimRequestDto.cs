using Claims.Domain;

namespace Claims.Api.Dto;

public record CreateClaimRequestDto(
    Guid CoverId,
    string Name,
    ClaimType Type,
    decimal DamageCost,
    DateTime Created
) : ICreateClaimRequest;
