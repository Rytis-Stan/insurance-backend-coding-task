using Claims.Application.Repositories;
using Claims.Domain;

namespace Claims.Application.Commands;

public class GetClaimByIdCommand : IGetClaimByIdCommand
{
    private readonly IClaimsRepository _claimsRepository;

    public GetClaimByIdCommand(IClaimsRepository claimsRepository)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task<Claim?> GetClaimByIdAsync(Guid id)
    {
        return await _claimsRepository.FindByIdAsync(id);
    }
}
