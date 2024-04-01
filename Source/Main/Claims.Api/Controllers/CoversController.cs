using Claims.Api.Dto;
using Claims.Auditing;
using Claims.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ICoversService _coversService;
    private readonly IPricingService _pricingService;
    private readonly ICoverAuditor _auditor;

    public CoversController(ICoversService coversService, IPricingService pricingService, ICoverAuditor auditor)
    {
        _coversService = coversService;
        _pricingService = pricingService;
        _auditor = auditor;
    }

    [HttpPost]
    public async Task<ActionResult<CoverDto>> CreateAsync(CreateCoverRequestDto request)
    {
        var cover = await _coversService.CreateCoverAsync(ToDomainRequest(request));
        _auditor.AuditCoverPost(cover.Id);
        return Ok(ToDto(cover));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> GetAsync(Guid id)
    {
        var cover = ToDto(await _coversService.GetCoverAsync(id));
        return cover != null
            ? Ok(cover)
            : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverDto>>> GetAsync()
    {
        var covers = await _coversService.GetAllCoversAsync();
        return Ok(covers);
    }

    [HttpGet("Premium")]
    public ActionResult<decimal> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return Ok(_pricingService.CalculatePremium(startDate, endDate, coverType));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Cover?>> DeleteAsync(Guid id)
    {
        _auditor.AuditCoverDelete(id);
        var deletedCover = await _coversService.DeleteCoverAsync(id);
        return Ok(ToDto(deletedCover));
    }

    private CreateCoverRequest ToDomainRequest(CreateCoverRequestDto request)
    {
        return new CreateCoverRequest(request.StartDate, request.EndDate, request.Type);
    }

    private CoverDto? ToDto(Cover? cover)
    {
        return cover != null
            ? new CoverDto(cover.Id, cover.StartDate, cover.EndDate, cover.Type, cover.Premium)
            : null;
    }
}
