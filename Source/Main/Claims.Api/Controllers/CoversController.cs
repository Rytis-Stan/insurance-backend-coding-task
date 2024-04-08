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
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CreateCoverResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCoverAsync([FromServices] ICommand<CreateCoverArgs, CreateCoverResult> command, CreateCoverRequest request)
    {
        var response = (await command.ExecuteAsync(request.ToCommandArgs())).ToResponse();
        _auditor.AuditPost(response.Cover.Id);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetCoverResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCoverAsync([FromServices] ICommand<GetCoverArgs, GetCoverResult> command, Guid id)
    {
        var response = (await command.ExecuteAsync(new GetCoverArgs(id))).ToResponse();
        return response.Cover != null
            ? Ok(response)
            : NotFound();
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(GetCoversResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCoversAsync([FromServices] ICommandWithNoArgs<GetAllCoversResult> command)
    {
        var response = (await command.ExecuteAsync()).ToResponse();
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCoverAsync([FromServices] ICommandWithNoResult<DeleteCoverArgs> command, Guid id)
    {
        await command.ExecuteAsync(new DeleteCoverArgs(id));
        _auditor.AuditDelete(id);
        return NoContent();
    }

    [HttpGet("Premium")]
    [ProducesResponseType(typeof(GetCoverPremiumResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCoverPremiumAsync([FromServices] ICommand<GetCoverPremiumArgs, GetCoverPremiumResult> command,
        DateOnly startDate, DateOnly endDate, CoverTypeDto coverType)
    {
        var response = (await command.ExecuteAsync(new GetCoverPremiumArgs(startDate, endDate, coverType.ToDomainEnum()))).ToResponse();
        return Ok(response);
    }
}
