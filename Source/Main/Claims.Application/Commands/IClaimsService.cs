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


public record CreateClaimRequest(
    Guid CoverId,
    string Name,
    ClaimType Type,
    decimal DamageCost,
    DateTime Created
);
public record CreateClaimResponse(Claim Claim);
// public interface ICreateClaimCommand : ICommand<CreateClaimRequest, CreateClaimResponse>
// {
// }

public record GetClaimByIdRequest(Guid Id);
public record GetClaimByIdResponse(Claim? Claim);
// public interface IGetClaimByIdCommand : ICommand<GetClaimByIdRequest, GetClaimByIdResponse>
// {
// }

public record GetAllClaimsResponse(IEnumerable<Claim> Claims);
public interface IGetAllClaimsCommand : INoParametersCommand<GetAllClaimsResponse>
{
}

public record DeleteClaimRequest(Guid Id);
public interface IDeleteClaimCommand : INoResultsCommand<DeleteClaimRequest>
{
}
