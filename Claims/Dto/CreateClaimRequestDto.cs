using Claims.Domain;

namespace Claims.Dto;

public class CreateClaimRequestDto : ICreateClaimRequest
{
    public Guid CoverId { get; init; }
    public string Name { get; init; }
    public ClaimType Type { get; init; }
    public decimal DamageCost { get; init; }
}