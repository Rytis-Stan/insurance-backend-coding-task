namespace Claims.Domain;

public interface INewClaimInfo
{
    Guid CoverId { get; }
    string Name { get; }
    ClaimType Type { get; }
    decimal DamageCost { get; }
}
