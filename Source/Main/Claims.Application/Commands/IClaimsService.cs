using Claims.Domain;

namespace Claims.Application.Commands;

public interface ICommand<in TRequest, TResponse>
{
    Task<TResponse> ExecuteAsync(TRequest request);
}

public interface INoParametersCommand<TResponse>
{
    Task<TResponse> ExecuteAsync();
}

public interface INoResultsCommand<in TRequest>
{
    Task ExecuteAsync(TRequest request);
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

public interface IGetAllClaimsCommand : INoParametersCommand<GetAllClaimsResponse>
{
    // Task<IEnumerable<Claim>> ExecuteAsync();
}

public record DeleteClaimRequest(Guid Id);

public interface IDeleteClaimCommand : INoResultsCommand<DeleteClaimRequest>
{
    // Task<Claim?> ExecuteAsync(DeleteClaimRequest request);
}
