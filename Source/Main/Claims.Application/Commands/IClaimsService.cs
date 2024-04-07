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

public interface ICreateClaimCommand : ICommand<CreateClaimRequest, CreateClaimResponse>
{
    // Task<Claim> ExecuteAsync(CreateClaimRequest request);
}

public record GetClaimByIdRequest(Guid Id);

public record GetClaimByIdResponse(Claim? Claim);

public interface IGetClaimByIdCommand : ICommand<GetClaimByIdRequest, GetClaimByIdResponse>
{
    // Task<Claim?> ExecuteAsync(GetClaimByIdRequest request);
}

public record GetAllClaimsResponse(IEnumerable<Claim> Claims);

public interface IGetAllClaimsCommand : ICommand<GetAllClaimsResponse>
{
    // Task<IEnumerable<Claim>> ExecuteAsync();
}

public record DeleteClaimRequest(Guid Id);

public interface IDeleteClaimCommand : ICommand<DeleteClaimRequest, Claim?>
{
    // Task<Claim?> ExecuteAsync(DeleteClaimRequest request);
}
