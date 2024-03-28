namespace Claims.Domain;

public interface IClaimsRepository
{
    Task<IEnumerable<Claim>> GetAllClaimsAsync();
    Task<Claim?> GetClaimAsync(Guid id);
    Task<Claim> AddItemAsync(Claim item);
    Task<Claim> DeleteItemAsync(Guid id);
}