using Claims.Api.Contracts.Dto;

namespace Claims.Api.Contracts.Messages;

public record CreateClaimRequest(
    Guid CoverId,
    string Name,
    ClaimDtoType Type,
    decimal DamageCost,
    DateTime Created
);
