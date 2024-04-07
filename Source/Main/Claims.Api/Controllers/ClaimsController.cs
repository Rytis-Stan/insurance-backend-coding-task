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
    public async Task<ActionResult<ClaimDto>> CreateClaimAsync([FromServices] ICommand<CreateClaimRequest, CreateClaimResponse> command, CreateClaimRequestDto request)
    {
        var response = await command.ExecuteAsync(request.ToDomainRequest());
        var claim = response.Claim;
        _auditor.AuditPost(claim.Id);
        return Ok(claim.ToDto());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClaimDto>> GetClaimAsync([FromServices] ICommand<GetClaimByIdRequest, GetClaimByIdResponse> command, Guid id)
    {
        var response = await command.ExecuteAsync(new GetClaimByIdRequest(id));
        var claim = response.Claim;
        return claim != null
            ? Ok(claim)
            : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetClaimsAsync([FromServices] ICommandWithNoParameters<GetAllClaimsResponse> command)
    {
        var response = await command.ExecuteAsync();
        var claims = response.Claims;
        return Ok(claims.Select(x => x.ToDto()));
    }

    [HttpDelete("{id}")]
    // TODO: Add proper attributes.
    //[ProducesResponseType(typeof(MyClass), (int)HttpStatusCode.Accepted)]
    public async Task<ActionResult> DeleteClaimAsync([FromServices] ICommandWithNoResults<DeleteClaimRequest> command, Guid id)
    {
        await command.ExecuteAsync(new DeleteClaimRequest(id));
        _auditor.AuditDelete(id);
        return NoContent();
    }
}
