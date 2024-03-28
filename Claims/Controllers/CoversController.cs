using Claims.Domain;
using Claims.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ICoversService _coversService;

    public CoversController(ICoversService coversService)
    {
        _coversService = coversService;
    }

    [HttpPost("Premium")]
    public async Task<ActionResult> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return Ok(_coversService.ComputePremium(startDate, endDate, coverType));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var covers = await _coversService.GetAllCoversAsync();
        return Ok(covers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        var cover = await _coversService.GetCoverAsync(id);
        return cover != null
            ? Ok(cover)
            : NotFound();
    }
    
    [HttpPost]
    public async Task<ActionResult<Cover>> CreateAsync(CreateCoverRequestDto request)
    {
        var cover = await _coversService.CreateCoverAsync(request);
        return Ok(cover);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(string id)
    {
        return _coversService.DeleteCoverAsync(id);
    }
}
