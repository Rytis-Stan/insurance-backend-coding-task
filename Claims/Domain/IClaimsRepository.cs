namespace Claims.Domain;

public interface IClaimsRepository
{
    Task<IEnumerable<Claim>> GetAllClaimsAsync();
    Task<Claim?> GetClaimAsync(string id);
    Task AddItemAsync(Claim item);
    Task DeleteItemAsync(string id);
}