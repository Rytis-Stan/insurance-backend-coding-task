using System.ComponentModel.DataAnnotations;
using Auditing.Auditors;
using Claims.Api.Contracts.Messages;
using Claims.Api.DependencyInjection;
using Claims.Application.Commands;
using Claims.Application.Commands.CreateClaim;
using Claims.Application.Commands.DeleteClaim;
using Claims.Application.Commands.GetClaim;
using Claims.Application.Commands.GetClaims;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IHttpRequestAuditor _auditor;

    public ClaimsController(ClaimAuditorSource auditorSource)
        : this(auditorSource.Obj)
    {
    }

    private ClaimsController(IHttpRequestAuditor auditor)
    {
        _auditor = auditor;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CreateClaimResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateClaimAsync([FromServices] ICommand<CreateClaimArgs, CreateClaimResult> command, [Required] CreateClaimRequest request)
    {
        var response = (await command.ExecuteAsync(request.ToCommandArgs())).ToResponse();
        _auditor.AuditPost(response.Claim.Id);
        return CreatedAtAction(nameof(GetClaimAsync), new { response.Claim.Id }, response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetClaimResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClaimAsync([FromServices] ICommand<GetClaimArgs, GetClaimResult> command, Guid id)
    {
        var response = (await command.ExecuteAsync(new GetClaimArgs(id))).ToResponse();
        return response.Claim != null
            ? Ok(response)
            : NotFound();
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetClaimsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClaimsAsync([FromServices] ICommandWithNoArgs<GetClaimsResult> command)
    {
        var response = (await command.ExecuteAsync()).ToResponse();
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteClaimAsync([FromServices] ICommandWithNoResult<DeleteClaimArgs> command, Guid id)
    {
        await command.ExecuteAsync(new DeleteClaimArgs(id));
        _auditor.AuditDelete(id);
        return NoContent();
    }
}
