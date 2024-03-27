using Claims.Auditing;
using Claims.Domain;
using Claims.Dto;
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

    [HttpPost("Premium")]
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
    public async Task<ActionResult> CreateAsync(CreateCoverRequestDto cover)
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
