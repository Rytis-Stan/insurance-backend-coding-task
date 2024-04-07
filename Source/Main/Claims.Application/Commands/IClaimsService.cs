using Claims.Domain;

namespace Claims.Application.Commands;

public record CreateClaimRequest(
    Guid CoverId,
    string Name,
    ClaimType Type,
    decimal DamageCost,
    DateTime Created
);
public record CreateClaimResponse(Claim Claim);

public record GetClaimByIdRequest(Guid Id);
public record GetClaimByIdResponse(Claim? Claim);


public record GetAllClaimsResponse(IEnumerable<Claim> Claims);
// public interface IGetAllClaimsCommand : INoParametersCommand<GetAllClaimsResponse>
// {
// }

public record DeleteClaimRequest(Guid Id);
// public interface IDeleteClaimCommand : INoResultsCommand<DeleteClaimRequest>
// {
// }
