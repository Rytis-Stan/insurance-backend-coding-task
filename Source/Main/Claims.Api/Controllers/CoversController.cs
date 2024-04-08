using Claims.Api.Dto;
using Claims.Application;
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
    public async Task<ActionResult<CoverDto>> CreateCoverAsync([FromServices] ICommand<CreateCoverArgs, CreateCoverResult> command,
        CreateCoverRequestDto request)
    {
        var response = await command.ExecuteAsync(request.ToCommandArgs());
        var cover = response.Cover.ToDto();
        _auditor.AuditPost(cover.Id);
        return Ok(cover);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> GetCoverAsync([FromServices] ICommand<GetCoverArgs, GetCoverResult> command, Guid id)
    {
        var response = await command.ExecuteAsync(new GetCoverArgs(id));
        var cover = response.Cover.ToDto();
        return cover != null
            ? Ok(cover)
            : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverDto>>> GetCoversAsync([FromServices] ICommandWithNoArgs<GetAllCoversResult> command)
    {
        var response = await command.ExecuteAsync();
        var covers = response.Covers.Select(x => x.ToDto());
        return Ok(covers);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCoverAsync([FromServices] ICommandWithNoResult<DeleteCoverArgs> command, Guid id)
    {
        await command.ExecuteAsync(new DeleteCoverArgs(id));
        _auditor.AuditDelete(id);
        return NoContent();
    }

    [HttpGet("Premium")]
    public async Task<ActionResult<decimal>> GetCoverPremiumAsync([FromServices] ICommand<GetCoverPremiumArgs, GetCoverPremiumResult> command,
        DateOnly startDate, DateOnly endDate, CoverTypeDto coverType)
    {
        var response = await command.ExecuteAsync(new GetCoverPremiumArgs(startDate, endDate, coverType.ToDomainEnum()));
        return Ok(response.Premium);
    }
}
