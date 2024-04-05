using Claims.Domain;

namespace Claims.Application.Commands;

public interface ICreateCoverCommand : ICommand<CreateCoverRequest, Cover>
{
    // Task<Cover> ExecuteAsync(CreateCoverRequest request);
}

public record GetCoverRequest(Guid Id);

public interface IGetCoverCommand
{
    Task<Cover?> ExecuteAsync(GetCoverRequest request);
}

public interface IGetAllCoversCommand : ICommand<IEnumerable<Cover>>
{
    // Task<IEnumerable<Cover>> ExecuteAsync();
}

public record DeleteCoverRequest(Guid Id);

public interface IDeleteCoverCommand
{
    Task<Cover?> ExecuteAsync(DeleteCoverRequest request);
}
