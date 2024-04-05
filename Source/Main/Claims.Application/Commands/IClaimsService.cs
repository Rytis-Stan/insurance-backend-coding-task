using Claims.Domain;

namespace Claims.Application.Commands;

public interface ICommand<in TRequest, TResponse>
{
    Task<TResponse> ExecuteAsync(TRequest request);
}

public interface ICommand<TResponse>
{
    Task<TResponse> ExecuteAsync();
}

public interface ICreateClaimCommand : ICommand<CreateClaimRequest, Claim>
{
    // Task<Claim> ExecuteAsync(CreateClaimRequest request);
}

public record GetClaimByIdRequest(Guid Id);

public interface IGetClaimByIdCommand : ICommand<GetClaimByIdRequest, Claim?>
{
    // Task<Claim?> ExecuteAsync(GetClaimByIdRequest request);
}

public interface IGetAllClaimsCommand : ICommand<IEnumerable<Claim>>
{
    // Task<IEnumerable<Claim>> ExecuteAsync();
}

public record DeleteClaimRequest(Guid Id);

public interface IDeleteClaimCommand : ICommand<DeleteClaimRequest, Claim?>
{
    // Task<Claim?> ExecuteAsync(DeleteClaimRequest request);
}
