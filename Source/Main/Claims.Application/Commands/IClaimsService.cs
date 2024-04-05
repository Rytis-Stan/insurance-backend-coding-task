using Claims.Domain;

namespace Claims.Application.Commands;

public interface ICommand<in TRequest, TResponse>
{
    Task<TResponse> ExecuteAsync(TRequest request);
}

public interface ICreateClaimCommand : ICommand<CreateClaimRequest, Claim>
{
    // Task<Claim> ExecuteAsync(CreateClaimRequest request);
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
