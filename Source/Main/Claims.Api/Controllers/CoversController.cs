using Claims.Api.Dto;
using Claims.Application.Commands;
using Claims.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ICoversService _coversService;
    private readonly IPricingService _pricingService;
    private readonly ICoverAuditor _coverAuditor;

    public CoversController(ICoversService coversService, IPricingService pricingService, ICoverAuditor coverAuditor)
    {
        _coversService = coversService;
        _pricingService = pricingService;
        _coverAuditor = coverAuditor;
    }

    [HttpPost]
    public async Task<ActionResult<CoverDto>> CreateAsync(CreateCoverRequestDto request)
    {
        var cover = await _coversService.CreateCoverAsync(request.ToDomainRequest());
        _coverAuditor.AuditPost(cover.Id);
        return Ok(cover.ToDto());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> GetAsync(Guid id)
    {
        var cover = (await _coversService.GetCoverAsync(id)).ToDto();
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
    public ActionResult<decimal> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverTypeDto coverType)
    {
        return Ok(_pricingService.CalculatePremium(startDate, endDate, coverType.ToDomainEnum()));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<CoverDto?>> DeleteAsync(Guid id)
    {
        _coverAuditor.AuditDelete(id);
        var deletedCover = await _coversService.DeleteCoverAsync(id);
        return Ok(deletedCover.ToDto());
    }
}
