using Claims.Api.Dto;
using Claims.Application.Commands;
using Claims.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ICoverAuditor _coverAuditor;

    public CoversController(ICoverAuditor coverAuditor)
    {
        _coverAuditor = coverAuditor;
    }

    [HttpPost]
    public async Task<ActionResult<CoverDto>> CreateAsync([FromServices] ICreateCoverCommand command, CreateCoverRequestDto request)
    {
        var response = await command.ExecuteAsync(request.ToDomainRequest());
        var cover = response.Cover;
        _coverAuditor.AuditPost(cover.Id);
        return Ok(cover.ToDto());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> GetAsync([FromServices] IGetCoverCommand command, Guid id)
    {
        var response = await command.ExecuteAsync(new GetCoverRequest(id));
        var cover = response.Cover;
        return cover != null
            ? Ok(cover.ToDto())
            : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverDto>>> GetAsync([FromServices] IGetAllCoversCommand command)
    {
        var response = await command.ExecuteAsync();
        var covers = response.Covers;
        return Ok(covers.Select(x => x.ToDto()));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync([FromServices] IDeleteCoverCommand command, Guid id)
    {
        await command.ExecuteAsync(new DeleteCoverRequest(id));
        _coverAuditor.AuditDelete(id);
        return NoContent();
    }

    [HttpGet("Premium")]
    public async Task<ActionResult<decimal>> GetCoverPremiumAsync([FromServices] IGetCoverPremiumCommand command,
        DateOnly startDate, DateOnly endDate, CoverTypeDto coverType)
    {
        var response = await command.ExecuteAsync(new GetCoverPremiumRequest(startDate, endDate, coverType.ToDomainEnum()));
        return Ok(response.Premium);
    }
}
