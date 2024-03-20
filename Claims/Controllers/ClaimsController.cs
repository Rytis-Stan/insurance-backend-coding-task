using Claims.Auditing;
using Claims.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimsService _claimsService;

    public ClaimsController(CosmosDbClaimsRepository cosmosDbClaimsRepository, AuditContext auditContext)
        : this(
            new ClaimsService(
                cosmosDbClaimsRepository, new Auditor(auditContext, new Clock())
            )
        )
    {
    }

    public ClaimsController(IClaimsService claimsService)
    {
        _claimsService = claimsService;
    }

    [HttpGet]
    public Task<IEnumerable<Claim>> GetAsync()
    {
        return _claimsService.GetAllClaimsAsync();
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(Claim claim)
    {
        await _claimsService.CreateClaimAsync(claim);
        return Ok(claim);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(string id)
    {
        return _claimsService.DeleteClaimAsync(id);
    }

    [HttpGet("{id}")]
    public Task<Claim> GetAsync(string id)
    {
        return _claimsService.GetClaimByIdAsync(id);
    }
}

public interface IClaimsService
{
    Task CreateClaimAsync(Claim claim);
    Task<IEnumerable<Claim>> GetAllClaimsAsync();
    Task DeleteClaimAsync(string id);
    Task<Claim> GetClaimByIdAsync(string id);
}

public class ClaimsService : IClaimsService
{
    private readonly IClaimsRepository _claimsRepository;
    private readonly IClaimAuditor _auditor;

    public ClaimsService(IClaimsRepository claimsRepository, IClaimAuditor auditor)
    {
        _claimsRepository = claimsRepository;
        _auditor = auditor;
    }

    public async Task CreateClaimAsync(Claim claim)
    {
        claim.Id = Guid.NewGuid().ToString();
        await _claimsRepository.AddItemAsync(claim);
        _auditor.AuditClaim(claim.Id, "POST");
    }

    public Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        return _claimsRepository.GetClaimsAsync();
    }

    public Task DeleteClaimAsync(string id)
    {
        _auditor.AuditClaim(id, "DELETE");
        var deletedClaim = _claimsRepository.DeleteItemAsync(id);
        return deletedClaim;
    }

    public Task<Claim> GetClaimByIdAsync(string id)
    {
        return _claimsRepository.GetClaimAsync(id);
    }
}

public interface IClaimsRepository
{
    Task<IEnumerable<Claim>> GetClaimsAsync();
    Task<Claim> GetClaimAsync(string id);
    Task AddItemAsync(Claim item);
    Task DeleteItemAsync(string id);
}

public class CosmosDbClaimsRepository : IClaimsRepository
{
    private readonly Container _container;

    public CosmosDbClaimsRepository(CosmosClient dbClient, string databaseName, string containerName)
    {
        ArgumentNullException.ThrowIfNull(dbClient, nameof(dbClient));
        _container = dbClient.GetContainer(databaseName, containerName);
    }

    public async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        return await GetAllClaimsAsync();
    }

    private async Task<IEnumerable<Claim>> GetAllClaimsAsync()
    {
        var query = _container.GetItemQueryIterator<Claim>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<Claim>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }
        return results;
    }

    public async Task<Claim> GetClaimAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Claim>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public Task AddItemAsync(Claim item)
    {
        return _container.CreateItemAsync(item, new PartitionKey(item.Id));
    }

    public Task DeleteItemAsync(string id)
    {
        return _container.DeleteItemAsync<Claim>(id, new PartitionKey(id));
    }
}