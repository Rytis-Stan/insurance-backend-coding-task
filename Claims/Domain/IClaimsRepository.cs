namespace Claims.Domain;

public interface IClaimsRepository
{
    Task<IEnumerable<Claim>> GetAllClaimsAsync();
    Task<Claim?> GetClaimAsync(string id);
    Task<Claim> AddItemAsync(Claim item);
    Task<Claim> DeleteItemAsync(string id);
}