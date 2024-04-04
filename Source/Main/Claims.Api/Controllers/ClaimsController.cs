using Claims.Api.Dto;
using Claims.Application;
using Claims.Auditing;
using Claims.Domain;
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
        var claim = await _claimsService.CreateClaimAsync(ToDomainRequest(request));
        _claimAuditor.AuditPost(claim.Id);
        return Ok(ToDto(claim));
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
        return Ok(claims.Select(ToDto));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ClaimDto?>> DeleteAsync(Guid id)
    {
        _claimAuditor.AuditDelete(id);
        var deletedClaim = await _claimsService.DeleteClaimAsync(id);
        return Ok(ToDto(deletedClaim));
    }

    private static CreateClaimRequest ToDomainRequest(CreateClaimRequestDto source)
    {
        var claimTypeDto = source.Type;
        return new CreateClaimRequest(source.CoverId, source.Name, ToDomainEnum(claimTypeDto), source.DamageCost, source.Created);
    }

    private static ClaimDto? ToDto(Claim? source)
    {
        return source != null
            ? new ClaimDto(source.Id, source.CoverId, source.Created, source.Name, ToDtoEnum(source.Type), source.DamageCost)
            : null;
    }

    private static ClaimType ToDomainEnum(ClaimTypeDto source)
    {
        return source switch
        {
            ClaimTypeDto.Collision => ClaimType.Collision,
            ClaimTypeDto.Grounding => ClaimType.Grounding,
            ClaimTypeDto.BadWeather => ClaimType.BadWeather,
            ClaimTypeDto.Fire => ClaimType.Fire,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }

    private static ClaimTypeDto ToDtoEnum(ClaimType source)
    {
        return source switch
        {
            ClaimType.Collision => ClaimTypeDto.Collision,
            ClaimType.Grounding => ClaimTypeDto.Grounding,
            ClaimType.BadWeather => ClaimTypeDto.BadWeather,
            ClaimType.Fire => ClaimTypeDto.Fire,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }
}