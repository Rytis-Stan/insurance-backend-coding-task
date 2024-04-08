using Claims.Api.Dto;
using Claims.Application.Commands;
using Claims.Application.Commands.CreateClaim;
using Claims.Application.Commands.DeleteClaim;
using Claims.Application.Commands.GetAllClaims;
using Claims.Application.Commands.GetClaimById;
using Claims.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimAuditor _auditor;

    public ClaimsController(IClaimAuditor auditor)
    {
        _auditor = auditor;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CreateClaimResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CreateClaimResponse>> CreateClaimAsync([FromServices] ICommand<CreateClaimArgs, CreateClaimResult> command, CreateClaimRequest request)
    {
        var response = (await command.ExecuteAsync(request.ToCommandArgs())).ToResponse();
        _auditor.AuditPost(response.Claim.Id);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetClaimResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetClaimResponse>> GetClaimAsync([FromServices] ICommand<GetClaimByIdArgs, GetClaimByIdResult> command, Guid id)
    {
        var response = (await command.ExecuteAsync(new GetClaimByIdArgs(id))).ToResponse();
        return response.Claim != null
            ? Ok(response)
            : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<GetClaimsResponse>> GetClaimsAsync([FromServices] ICommandWithNoArgs<GetAllClaimsResult> command)
    {
        var response = (await command.ExecuteAsync()).ToResponse();
        return Ok(response);
    }

    [HttpDelete("{id}")]
    // TODO: Add proper attributes.
    //[ProducesResponseType(typeof(MyClass), (int)HttpStatusCode.Accepted)]
    public async Task<ActionResult> DeleteClaimAsync([FromServices] ICommandWithNoResult<DeleteClaimArgs> command, Guid id)
    {
        await command.ExecuteAsync(new DeleteClaimArgs(id));
        _auditor.AuditDelete(id);
        return NoContent();
    }
}
