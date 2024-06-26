using System.ComponentModel.DataAnnotations;
using Auditing.Auditors;
using Claims.Api.Contracts.Dto;
using Claims.Api.Contracts.Messages;
using Claims.Api.DependencyInjection;
using Claims.Application.Commands;
using Claims.Application.Commands.CreateCover;
using Claims.Application.Commands.DeleteCover;
using Claims.Application.Commands.GetCover;
using Claims.Application.Commands.GetCoverPremium;
using Claims.Application.Commands.GetCovers;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly IHttpRequestAuditor _auditor;

    public CoversController(CoverAuditorSource auditorSource)
        : this(auditorSource.Obj)
    {
    }

    private CoversController(IHttpRequestAuditor auditor)
    {
        _auditor = auditor;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CreateCoverResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCoverAsync([FromServices] ICommand<CreateCoverArgs, CreateCoverResult> command, [Required] CreateCoverRequest request)
    {
        var response = (await command.ExecuteAsync(request.ToCommandArgs())).ToResponse();
        _auditor.AuditPost(response.Cover.Id);
        return CreatedAtAction(nameof(GetCoverAsync), new { response.Cover.Id }, response);
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
    public async Task<IActionResult> GetCoversAsync([FromServices] ICommandWithNoArgs<GetCoversResult> command)
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
        [Required] DateOnly startDate, [Required] DateOnly endDate, [Required] CoverDtoType coverType)
    {
        var response = (await command.ExecuteAsync(new GetCoverPremiumArgs(startDate, endDate, coverType.ToDomainEnum()))).ToResponse();
        return Ok(response);
    }
}
