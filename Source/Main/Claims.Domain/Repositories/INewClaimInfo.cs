namespace Claims.Domain.Repositories;

public interface INewClaimInfo
{
    Guid CoverId { get; }
    string Name { get; }
    ClaimType Type { get; }
    decimal DamageCost { get; }
    DateTime Created { get; }
}
