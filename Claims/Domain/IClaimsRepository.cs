namespace Claims.Domain;

public interface IClaimsRepository
{
    Task<Claim> AddItemAsync(Claim item);
    Task<Claim?> GetClaimAsync(Guid id);
    Task<IEnumerable<Claim>> GetAllClaimsAsync();
    Task<Claim> DeleteItemAsync(Guid id);
}
