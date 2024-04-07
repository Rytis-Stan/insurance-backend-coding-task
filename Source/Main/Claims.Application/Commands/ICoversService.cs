using Claims.Domain;

namespace Claims.Application.Commands;

public record CreateCoverRequest(DateOnly StartDate, DateOnly EndDate, CoverType Type);
public record CreateCoverResponse(Cover Cover);
// public interface ICreateCoverCommand : ICommand<CreateCoverRequest, CreateCoverResponse>
// {
// }

public record GetCoverRequest(Guid Id);
public record GetCoverResponse(Cover? Cover);
// public interface IGetCoverCommand : ICommand<GetCoverRequest, GetCoverResponse>
// {
// }

public record GetAllCoversResponse(IEnumerable<Cover> Covers);
public interface IGetAllCoversCommand : INoParametersCommand<GetAllCoversResponse>
{
}

public record DeleteCoverRequest(Guid Id);
// public interface IDeleteCoverCommand : INoResultsCommand<DeleteCoverRequest>
// {
// }
