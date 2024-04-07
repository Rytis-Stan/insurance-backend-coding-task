using Claims.Api.Dto;
using Claims.Application.Commands;
using Claims.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly ICreateClaimCommand _createClaimCommand;
    private readonly IGetClaimByIdCommand _getClaimByIdCommand;
    private readonly IGetAllClaimsCommand _getAllClaimsCommand;
    private readonly IDeleteClaimCommand _deleteClaimCommand;
    private readonly IClaimAuditor _claimAuditor;

    public ClaimsController(ICreateClaimCommand createClaimCommand, IGetClaimByIdCommand getClaimByIdCommand, IGetAllClaimsCommand getAllClaimsCommand, IDeleteClaimCommand deleteClaimCommand, IClaimAuditor claimAuditor)
    {
        _createClaimCommand = createClaimCommand;
        _getClaimByIdCommand = getClaimByIdCommand;
        _getAllClaimsCommand = getAllClaimsCommand;
        _deleteClaimCommand = deleteClaimCommand;
        _claimAuditor = claimAuditor;
    }

    [HttpPost]
    public async Task<ActionResult<ClaimDto>> CreateAsync(CreateClaimRequestDto request)
    {
        var response = await _createClaimCommand.ExecuteAsync(request.ToDomainRequest());
        var claim = response.Claim;
        _claimAuditor.AuditPost(claim.Id);
        return Ok(claim.ToDto());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClaimDto>> GetAsync(Guid id)
    {
        var response = await _getClaimByIdCommand.ExecuteAsync(new GetClaimByIdRequest(id));
        var claim = response.Claim;
        return claim != null
            ? Ok(claim)
            : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetAsync()
    {
        var response = await _getAllClaimsCommand.ExecuteAsync();
        var claims = response.Claims;
        return Ok(claims.Select(x => x.ToDto()));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        await _deleteClaimCommand.ExecuteAsync(new DeleteClaimRequest(id));
        _claimAuditor.AuditDelete(id);
        return NoContent();
    }
}
