using System.Net;
using Claims.Auditing;
using Claims.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ICoversService _coversService;

    public CoversController(CosmosClient cosmosClient, AuditContext auditContext)
        : this(
            new CoversService(
                cosmosClient?.GetContainer("ClaimDb", "Cover") ?? throw new ArgumentNullException(nameof(cosmosClient)),
                new Auditor(auditContext, new Clock())
            )
        )
    {
    }

    private CoversController(ICoversService coversService)
    {
        _coversService = coversService;
    }

    [HttpPost]
    public async Task<ActionResult> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return Ok(_coversService.ComputePremium(startDate, endDate, coverType));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var covers = await _coversService.GetAllCoversAsync();
        return Ok(covers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        var cover = await _coversService.GetCoverAsync(id);
        return cover != null
            ? Ok(cover)
            : NotFound();
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        await _coversService.CreateCoverAsync(cover);
        return Ok(cover);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(string id)
    {
        return _coversService.DeleteCoverAsync(id);
    }
}

public interface ICoversService
{
    Task CreateCoverAsync(Cover cover);
    decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType);
    Task<Cover?> GetCoverAsync(string id);
    Task<List<Cover>> GetAllCoversAsync();
    Task<ItemResponse<Cover>> DeleteCoverAsync(string id);
}

public class CoversService : ICoversService
{
    private readonly Container _container;
    private readonly ICoverAuditor _auditor;

    public CoversService(Container container, ICoverAuditor auditor)
    {
        _container = container;
        _auditor = auditor;
    }

    public async Task CreateCoverAsync(Cover cover)
    {
        cover.Id = Guid.NewGuid().ToString();
        cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
        await _container.CreateItemAsync(cover, new PartitionKey(cover.Id));
        _auditor.AuditCover(cover.Id, "POST");
    }

    public decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var multiplier = Multiplier(coverType);

        var premiumPerDay = 1250 * multiplier;
        var insuranceLength = endDate.DayNumber - startDate.DayNumber;
        var totalPremium = 0m;

        for (var i = 0; i < insuranceLength; i++)
        {
            if (i < 30)
            {
                totalPremium += premiumPerDay;
            }
            if (i < 180 && coverType == CoverType.Yacht)
            {
                totalPremium += premiumPerDay - premiumPerDay * 0.05m;
            }
            else if (i < 180)
            {
                totalPremium += premiumPerDay - premiumPerDay * 0.02m;
            }
            if (i < 365 && coverType != CoverType.Yacht)
            {
                totalPremium += premiumPerDay - premiumPerDay * 0.03m;
            }
            else if (i < 365)
            {
                totalPremium += premiumPerDay - premiumPerDay * 0.08m;
            }
        }

        return totalPremium;
    }

    private decimal Multiplier(CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => 1.1m,
            CoverType.PassengerShip => 1.2m,
            CoverType.Tanker => 1.5m,
            _ => 1.3m
        };
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

    public async Task<List<Cover>> GetAllCoversAsync()
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
        _auditor.AuditCover(id, "DELETE");
        var deletedCover = _container.DeleteItemAsync<Cover>(id, new(id));
        return deletedCover;
    }
}