using Claims.Domain;
using Claims.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimsService _claimsService;

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
    public async Task<ActionResult> CreateAsync(CreateClaimRequestDto claim)
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
    public async Task<ActionResult<Claim>> GetAsync(string id)
    {
        var claim = await _claimsService.GetClaimByIdAsync(id);
        return claim != null
            ? Ok(claim)
            : NotFound();
    }
}