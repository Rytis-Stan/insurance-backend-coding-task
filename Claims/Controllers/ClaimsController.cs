using Claims.Auditing;
using Claims.Domain;
using Claims.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimsService _claimsService;
    private readonly IClaimAuditor _auditor;

    public ClaimsController(IClaimsService claimsService, IClaimAuditor auditor)
    {
        _claimsService = claimsService;
        _auditor = auditor;
    }

    [HttpGet]
    public Task<IEnumerable<Claim>> GetAsync()
    {
        return _claimsService.GetAllClaimsAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Claim>> CreateAsync(CreateClaimRequestDto request)
    {
        var claim = await _claimsService.CreateClaimAsync(request);
        _auditor.AuditClaim(claim.Id, "POST");
        return Ok(claim);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(string id)
    {
        _auditor.AuditClaim(id, "DELETE");
        return _claimsService.DeleteClaimAsync(id);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Claim>> GetAsync(string id)
    {
        var claim = await _claimsService.GetClaimByIdAsync(id);
        return claim != null
            ? Ok(claim)
            : NotFound();
    }
}