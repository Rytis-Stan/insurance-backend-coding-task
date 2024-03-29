using Claims.Domain;

namespace Claims.Tests;

public class CoversRepositoryStub : ICoversRepository
{
    private readonly Dictionary<Guid, Cover?> _coversByIds;

    public CoversRepositoryStub()
        : this(new Dictionary<Guid, Cover?>())
    {
    }

    public CoversRepositoryStub(Guid coverId, Cover? coverToReturn)
        : this(new Dictionary<Guid, Cover?> {{ coverId, coverToReturn }})
    {
    }

    public CoversRepositoryStub(Dictionary<Guid, Cover?> coversByIds)
    {
        _coversByIds = coversByIds;
    }

    public Task<Cover> AddAsync(INewCoverInfo item)
    {
        throw new NotImplementedException();
    }

    public Task<Cover?> GetByIdAsync(Guid id)
    {
        _coversByIds.TryGetValue(id, out var cover);
        return Task.FromResult(cover);
    }

    public Task<IEnumerable<Cover>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Cover> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
