using Claims.Api.Auditing;
using Claims.Api.Dto;
using Claims.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

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

    [HttpPost]
    public async Task<ActionResult<Claim>> CreateAsync(CreateClaimRequestDto request)
    {
        var claim = await _claimsService.CreateClaimAsync(request);
        _auditor.AuditClaimPost(claim.Id);
        return Ok(claim);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Claim>> GetAsync(Guid id)
    {
        var claim = await _claimsService.GetClaimByIdAsync(id);
        return claim != null
            ? Ok(claim)
            : NotFound();
    }

    [HttpGet]
    public Task<IEnumerable<Claim>> GetAsync()
    {
        return _claimsService.GetAllClaimsAsync();
    }

    [HttpDelete("{id}")]
    public async Task<Claim?> DeleteAsync(Guid id)
    {
        _auditor.AuditClaimDelete(id);
        return await _claimsService.DeleteClaimAsync(id);
    }
}