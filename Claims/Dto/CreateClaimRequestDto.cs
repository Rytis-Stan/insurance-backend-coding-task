using Claims.Domain;

namespace Claims.Dto;

public class CreateClaimRequestDto : ICreateClaimRequest
{
    public Guid CoverId { get; set; }
    public string Name { get; set; }
    public ClaimType Type { get; set; }
    public decimal DamageCost { get; set; }
}
