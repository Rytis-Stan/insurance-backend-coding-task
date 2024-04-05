using Claims.Domain;

namespace Claims.Application.Commands;

public interface ICreateCoverCommand
{
    Task<Cover> CreateCoverAsync(CreateCoverRequest request);
}

public interface IGetCoverCommand
{
    Task<Cover?> GetCoverAsync(Guid id);
}

public interface IGetAllCoversCommand
{
    Task<IEnumerable<Cover>> GetAllCoversAsync();
}

public interface IDeleteCoverCommand
{
    Task<Cover?> DeleteCoverAsync(Guid id);
}
