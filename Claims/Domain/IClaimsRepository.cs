namespace Claims.Domain;

public interface IClaimsRepository
{
    Task<Claim> AddItemAsync(INewClaimInfo item);
    Task<Claim?> GetClaimAsync(Guid id);
    Task<IEnumerable<Claim>> GetAllClaimsAsync();
    Task<Claim> DeleteItemAsync(Guid id);
}

public interface INewClaimInfo
{
    Guid CoverId { get; }
    string Name { get; }
    ClaimType Type { get; }
    decimal DamageCost { get; }
}
