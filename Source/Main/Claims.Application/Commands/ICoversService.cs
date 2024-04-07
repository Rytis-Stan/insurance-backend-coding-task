using Claims.Domain;

namespace Claims.Application.Commands;

public interface ICreateCoverCommand : ICommand<CreateCoverRequest, CreateCoverResponse>
{
    // Task<Cover> ExecuteAsync(CreateCoverRequest request);
}

public record GetCoverRequest(Guid Id);

public record GetCoverResponse(Cover? Cover);

public interface IGetCoverCommand : ICommand<GetCoverRequest, GetCoverResponse>
{
    // Task<Cover?> ExecuteAsync(GetCoverRequest request);
}

public record GetAllCoversResponse(IEnumerable<Cover> Covers);

public interface IGetAllCoversCommand : ICommand<GetAllCoversResponse>
{
    // Task<IEnumerable<Cover>> ExecuteAsync();
}

public record DeleteCoverRequest(Guid Id);

public interface IDeleteCoverCommand : ICommand<DeleteCoverRequest, Cover?>
{
    // Task<Cover?> ExecuteAsync(DeleteCoverRequest request);
}
