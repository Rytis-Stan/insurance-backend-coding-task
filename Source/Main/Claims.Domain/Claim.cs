namespace Claims.Domain;

public class Claim
{
    public required Guid Id { get; init; }
    public required Guid CoverId { get; init; }
    public required string Name { get; init; }
    public required ClaimType Type { get; init; }
    public required decimal DamageCost { get; init; }
    public required DateTime Created { get; init; }
}