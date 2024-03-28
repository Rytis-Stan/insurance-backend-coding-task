namespace Claims.Domain;

public class Claim
{
    public string Id { get; init; }
    public string CoverId { get; init; }
    public DateTime Created { get; init; }
    public string Name { get; init; }
    public ClaimType Type { get; init; }
    public decimal DamageCost { get; init; }
}

public enum ClaimType
{
    Collision = 0,
    Grounding = 1,
    BadWeather = 2,
    Fire = 3
}