using Claims.Auditing;
using Claims.Domain;
using Claims.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ICoversService _coversService;
    private readonly ICoverAuditor _auditor;

    public CoversController(ICoversService coversService, ICoverAuditor auditor)
    {
        _coversService = coversService;
        _auditor = auditor;
    }

    [HttpPost]
    public async Task<ActionResult<Cover>> CreateAsync(CreateCoverRequestDto request)
    {
        var cover = await _coversService.CreateCoverAsync(request);
        _auditor.AuditCover(cover.Id, "POST");
        return Ok(cover);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(Guid id)
    {
        var cover = await _coversService.GetCoverAsync(id);
        return cover != null
            ? Ok(cover)
            : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var covers = await _coversService.GetAllCoversAsync();
        return Ok(covers);
    }

    [HttpDelete("{id}")]
    public Task<Cover> DeleteAsync(Guid id)
    {
        _auditor.AuditCover(id, "DELETE");
        return _coversService.DeleteCoverAsync(id);
    }

    [HttpPost("Premium")]
    public async Task<ActionResult> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return Ok(_coversService.ComputePremium(startDate, endDate, coverType));
    }
}
