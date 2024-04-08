using Azure.Core;
using Claims.Api.Dto;
using Claims.Application;
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
    public async Task<ActionResult<ClaimDto>> CreateClaimAsync([FromServices] ICommand<CreateClaimArgs, CreateClaimResult> command, CreateClaimRequestDto request)
    {
        var response = await command.ExecuteAsync(request.ToCommandArgs());
        var claim = response.Claim.ToDto();
        _auditor.AuditPost(claim.Id);
        return Ok(claim);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClaimDto>> GetClaimAsync([FromServices] ICommand<GetClaimByIdArgs, GetClaimByIdResult> command, Guid id)
    {
        var response = await command.ExecuteAsync(new GetClaimByIdArgs(id));
        var claim = response.Claim.ToDto();
        return claim != null
            ? Ok(claim)
            : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetClaimsAsync([FromServices] ICommandWithNoArgs<GetAllClaimsResult> command)
    {
        var response = await command.ExecuteAsync();
        var claims = response.Claims.Select(x => x.ToDto());
        return Ok(claims);
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
