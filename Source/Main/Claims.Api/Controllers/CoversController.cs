using Claims.Api.Dto;
using Claims.Application.Commands;
using Claims.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ICreateCoverCommand _createCoverCommand;
    private readonly IGetCoverCommand _getCoverCommand;
    private readonly IGetAllCoversCommand _getAllCoversCommand;
    private readonly IDeleteCoverCommand _deleteCoverCommand;
    private readonly ICoverPricing _coverPricing;
    private readonly ICoverAuditor _coverAuditor;

    // TODO: Fix issue of too many controller constructor parameters (and do the same for the other controller)!
    public CoversController(ICreateCoverCommand createCoverCommand, IGetCoverCommand getCoverCommand, IGetAllCoversCommand getAllCoversCommand, IDeleteCoverCommand deleteCoverCommand, ICoverPricing coverPricing, ICoverAuditor coverAuditor)
    {
        _createCoverCommand = createCoverCommand;
        _getCoverCommand = getCoverCommand;
        _getAllCoversCommand = getAllCoversCommand;
        _deleteCoverCommand = deleteCoverCommand;
        _coverPricing = coverPricing;
        _coverAuditor = coverAuditor;
    }

    [HttpPost]
    public async Task<ActionResult<CoverDto>> CreateAsync(CreateCoverRequestDto request)
    {
        var cover = await _createCoverCommand.ExecuteAsync(request.ToDomainRequest());
        _coverAuditor.AuditPost(cover.Id);
        return Ok(cover.ToDto());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> GetAsync(Guid id)
    {
        var cover = (await _getCoverCommand.ExecuteAsync(new GetCoverRequest(id))).ToDto();
        return cover != null
            ? Ok(cover)
            : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverDto>>> GetAsync()
    {
        var covers = await _getAllCoversCommand.ExecuteAsync();
        return Ok(covers);
    }

    [HttpGet("Premium")]
    public ActionResult<decimal> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverTypeDto coverType)
    {
        return Ok(_coverPricing.CalculatePremium(startDate, endDate, coverType.ToDomainEnum()));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<CoverDto?>> DeleteAsync(Guid id)
    {
        var deletedCover = await _deleteCoverCommand.ExecuteAsync(new DeleteCoverRequest(id));
        _coverAuditor.AuditDelete(id);
        return Ok(deletedCover.ToDto());
    }
}
