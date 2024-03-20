namespace Claims.Domain;

public interface IClaimsRepository
{
    Task<IEnumerable<Claim>> GetClaimsAsync();
    Task<Claim> GetClaimAsync(string id);
    Task AddItemAsync(Claim item);
    Task DeleteItemAsync(string id);
}