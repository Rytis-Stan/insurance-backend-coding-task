using Claims.Domain;

namespace Claims.Application.Commands;

public interface ICreateCoverCommand
{
    Task<Cover> ExecuteAsync(CreateCoverRequest request);
}

public interface IGetCoverCommand
{
    Task<Cover?> ExecuteAsync(Guid id);
}

public interface IGetAllCoversCommand
{
    Task<IEnumerable<Cover>> ExecuteAsync();
}

public interface IDeleteCoverCommand
{
    Task<Cover?> ExecuteAsync(Guid id);
}
