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
    private readonly IGetCoverPremiumCommand _getCoverPremiumCommand;
    private readonly ICoverAuditor _coverAuditor;

    // TODO: Fix issue of too many controller constructor parameters (and do the same for the other controller)!
    public CoversController(ICreateCoverCommand createCoverCommand, IGetCoverCommand getCoverCommand, IGetAllCoversCommand getAllCoversCommand, IDeleteCoverCommand deleteCoverCommand, IGetCoverPremiumCommand getCoverPremiumCommand, ICoverAuditor coverAuditor)
    {
        _createCoverCommand = createCoverCommand;
        _getCoverCommand = getCoverCommand;
        _getAllCoversCommand = getAllCoversCommand;
        _deleteCoverCommand = deleteCoverCommand;
        _getCoverPremiumCommand = getCoverPremiumCommand;
        _coverAuditor = coverAuditor;
    }

    [HttpPost]
    public async Task<ActionResult<CoverDto>> CreateAsync(CreateCoverRequestDto request)
    {
        var response = await _createCoverCommand.ExecuteAsync(request.ToDomainRequest());
        var cover = response.Cover;
        _coverAuditor.AuditPost(cover.Id);
        return Ok(cover.ToDto());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> GetAsync(Guid id)
    {
        var response = await _getCoverCommand.ExecuteAsync(new GetCoverRequest(id));
        var cover = response.Cover;
        return cover != null
            ? Ok(cover.ToDto())
            : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverDto>>> GetAsync()
    {
        var response = await _getAllCoversCommand.ExecuteAsync();
        var covers = response.Covers;
        return Ok(covers.Select(x => x.ToDto()));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await _deleteCoverCommand.ExecuteAsync(new DeleteCoverRequest(id));
        _coverAuditor.AuditDelete(id);
        return NoContent();
    }

    [HttpGet("Premium")]
    public ActionResult<decimal> GetCoverPremiumAsync(DateOnly startDate, DateOnly endDate, CoverTypeDto coverType)
    {
        return Ok(_getCoverPremiumCommand.ExecuteAsync(new GetCoverPremiumRequest(startDate, endDate, coverType.ToDomainEnum())));
    }
}
