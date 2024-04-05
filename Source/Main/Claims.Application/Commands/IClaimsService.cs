using Claims.Domain;

namespace Claims.Application.Commands;

public interface ICreateClaimCommand
{
    Task<Claim> ExecuteAsync(CreateClaimRequest request);
}

public interface IGetClaimByIdCommand
{
    Task<Claim?> ExecuteAsync(Guid id);
}

public interface IGetAllClaimsCommand
{
    Task<IEnumerable<Claim>> ExecuteAsync();
}

public interface IDeleteClaimCommand
{
    Task<Claim?> ExecuteAsync(Guid id);
}
