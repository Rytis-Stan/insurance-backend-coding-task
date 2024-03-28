namespace Claims.Domain;

public interface IClaimsRepository
{
    Task<Claim> AddAsync(INewClaimInfo item);
    Task<Claim?> GetByIdAsync(Guid id);
    Task<IEnumerable<Claim>> GetAllAsync();
    Task<Claim> DeleteAsync(Guid id);
}

public interface INewClaimInfo
{
    Guid CoverId { get; }
    string Name { get; }
    ClaimType Type { get; }
    decimal DamageCost { get; }
}
