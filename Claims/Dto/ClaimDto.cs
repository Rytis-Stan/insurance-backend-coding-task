using Claims.Domain;

namespace Claims.Dto;

public class ClaimDto
{
    public string Id { get; init; }
    public string CoverId { get; init; }
    public DateTime Created { get; init; }
    public string Name { get; init; }
    public ClaimType Type { get; init; }
    public decimal DamageCost { get; init; }
}
