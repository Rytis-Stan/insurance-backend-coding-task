using Claims.Domain;

namespace Claims.Application.Commands;

public interface ICreateClaimCommand
{
    Task<Claim> CreateClaimAsync(CreateClaimRequest request);
}

public interface IGetClaimByIdCommand
{
    Task<Claim?> GetClaimByIdAsync(Guid id);
}

public interface IGetAllClaimsCommand
{
    Task<IEnumerable<Claim>> GetAllClaimsAsync();
}

public interface IDeleteClaimCommand
{
    Task<Claim?> DeleteClaimAsync(Guid id);
}

public interface IClaimsService : ICreateClaimCommand, IGetClaimByIdCommand, IGetAllClaimsCommand, IDeleteClaimCommand
{
}
