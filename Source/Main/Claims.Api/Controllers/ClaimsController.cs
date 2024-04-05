using Claims.Api.Dto;
using Claims.Application;
using Claims.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimsService _claimsService;
    private readonly IClaimAuditor _claimAuditor;

    public ClaimsController(IClaimsService claimsService, IClaimAuditor claimAuditor)
    {
        _claimsService = claimsService;
        _claimAuditor = claimAuditor;
    }

    [HttpPost]
    public async Task<ActionResult<ClaimDto>> CreateAsync(CreateClaimRequestDto request)
    {
        var claim = await _claimsService.CreateClaimAsync(request.ToDomainRequest());
        _claimAuditor.AuditPost(claim.Id);
        return Ok(claim.ToDto());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClaimDto>> GetAsync(Guid id)
    {
        var claim = await _claimsService.GetClaimByIdAsync(id);
        return claim != null
            ? Ok(claim)
            : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetAsync()
    {
        var claims = await _claimsService.GetAllClaimsAsync();
        return Ok(claims.Select(MappingExtensions.ToDto));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ClaimDto?>> DeleteAsync(Guid id)
    {
        _claimAuditor.AuditDelete(id);
        var deletedClaim = await _claimsService.DeleteClaimAsync(id);
        return Ok(deletedClaim.ToDto());
    }
}
