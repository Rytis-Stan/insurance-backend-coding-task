using Claims.Domain;

namespace Claims.Application.Commands;

public interface ICreateCoverCommand : ICommand<CreateCoverRequest, Cover>
{
    // Task<Cover> ExecuteAsync(CreateCoverRequest request);
}

public interface IGetCoverCommand
{
    Task<Cover?> ExecuteAsync(Guid id);
}

public interface IGetAllCoversCommand : ICommand<IEnumerable<Cover>>
{
    // Task<IEnumerable<Cover>> ExecuteAsync();
}

public interface IDeleteCoverCommand
{
    Task<Cover?> ExecuteAsync(Guid id);
}
