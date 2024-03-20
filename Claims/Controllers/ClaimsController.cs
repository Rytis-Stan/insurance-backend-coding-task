using Claims.Auditing;
using Claims.DataAccess;
using Claims.Domain;
using Claims.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimsService _claimsService;

    public ClaimsController(CosmosDbClaimsRepository claimsRepository, AuditContext auditContext)
        : this(
            new ClaimsService(
                claimsRepository, new Auditor(auditContext, new Clock())
            )
        )
    {
    }

    public ClaimsController(IClaimsService claimsService)
    {
        _claimsService = claimsService;
    }

    [HttpGet]
    public Task<IEnumerable<Claim>> GetAsync()
    {
        return _claimsService.GetAllClaimsAsync();
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(Claim claim)
    {
        await _claimsService.CreateClaimAsync(claim);
        return Ok(claim);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(string id)
    {
        return _claimsService.DeleteClaimAsync(id);
    }

    [HttpGet("{id}")]
    public Task<Claim> GetAsync(string id)
    {
        return _claimsService.GetClaimByIdAsync(id);
    }
}