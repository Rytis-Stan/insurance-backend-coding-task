using System.Net;
using Claims.Auditing;
using Microsoft.Azure.Cosmos;

namespace Claims.Domain;

public class CoversService : ICoversService
{
    private readonly Container _container;
    private readonly ICoverAuditor _auditor;

    public CoversService(Container container, ICoverAuditor auditor)
    {
        _container = container;
        _auditor = auditor;
    }

    public async Task CreateCoverAsync(ICreateCoverRequest request)
    {
        var cover = new Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Type = request.Type,
            Premium = Cover.ComputePremium(request.StartDate, request.EndDate, request.Type)
        };
        await _container.CreateItemAsync(cover, new PartitionKey(cover.Id));
        _auditor.AuditCover(cover.Id, "POST");
    }

    public decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return Cover.ComputePremium(startDate, endDate, coverType);
    }

    public async Task<Cover?> GetCoverAsync(string id)
    {
        return await CoversRepository.GetCoverAsync(id);
    }

    public async Task<IEnumerable<Cover>> GetAllCoversAsync()
    {
        return await CoversRepository.GetAllCoversAsync();
    }

    public async Task<ItemResponse<Cover>> DeleteCoverAsync(string id)
    {
        _auditor.AuditCover(id, "DELETE");
        return await CoversRepository.DeleteCoverAsync(id);
    }

    private ICoversRepository CoversRepository => new CoversRepository(_container);
}

public interface ICreateCoverRequest
{
    DateOnly StartDate { get; }
    DateOnly EndDate { get; }
    CoverType Type { get; }
}

public interface ICoversRepository
{
    Task<Cover?> GetCoverAsync(string id);
    Task<IEnumerable<Cover>> GetAllCoversAsync();
    Task<ItemResponse<Cover>> DeleteCoverAsync(string id);
}

public class CoversRepository : ICoversRepository
{
    private readonly Container _container;

    public CoversRepository(Container container)
    {
        _container = container;
    }

    public async Task<Cover?> GetCoverAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Cover>(id, new(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Cover>> GetAllCoversAsync()
    {
        var query = _container.GetItemQueryIterator<Cover>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<Cover>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }
        return results;
    }

    public Task<ItemResponse<Cover>> DeleteCoverAsync(string id)
    {
        return _container.DeleteItemAsync<Cover>(id, new(id));
    }
}