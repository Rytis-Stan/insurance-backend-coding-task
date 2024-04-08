using Claims.Api.Dto;
using Claims.Application.Commands;
using Claims.Application.Commands.CreateCover;
using Claims.Application.Commands.DeleteCover;
using Claims.Application.Commands.GetAllCovers;
using Claims.Application.Commands.GetCover;
using Claims.Application.Commands.GetCoverPremium;
using Claims.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ICoverAuditor _auditor;

    public CoversController(ICoverAuditor auditor)
    {
        _auditor = auditor;
    }

    [HttpPost]
    public async Task<ActionResult<CoverDto>> CreateCoverAsync([FromServices] ICommand<CreateCoverRequest, CreateCoverResponse> command,
        CreateCoverRequestDto request)
    {
        var response = await command.ExecuteAsync(request.ToDomainRequest());
        var cover = response.Cover.ToDto();
        _auditor.AuditPost(cover.Id);
        return Ok(cover);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> GetCoverAsync([FromServices] ICommand<GetCoverRequest, GetCoverResponse> command, Guid id)
    {
        var response = await command.ExecuteAsync(new GetCoverRequest(id));
        var cover = response.Cover.ToDto();
        return cover != null
            ? Ok(cover)
            : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverDto>>> GetCoversAsync([FromServices] ICommandWithNoParameters<GetAllCoversResponse> command)
    {
        var response = await command.ExecuteAsync();
        var covers = response.Covers.Select(x => x.ToDto());
        return Ok(covers);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCoverAsync([FromServices] ICommandWithNoResults<DeleteCoverRequest> command, Guid id)
    {
        await command.ExecuteAsync(new DeleteCoverRequest(id));
        _auditor.AuditDelete(id);
        return NoContent();
    }

    [HttpGet("Premium")]
    public async Task<ActionResult<decimal>> GetCoverPremiumAsync([FromServices] ICommand<GetCoverPremiumRequest, GetCoverPremiumResponse> command,
        DateOnly startDate, DateOnly endDate, CoverTypeDto coverType)
    {
        var response = await command.ExecuteAsync(new GetCoverPremiumRequest(startDate, endDate, coverType.ToDomainEnum()));
        return Ok(response.Premium);
    }
}
