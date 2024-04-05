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

    public async Task<Claim?> ExecuteAsync(GetClaimByIdRequest request)
    {
        return await _claimsRepository.FindByIdAsync(request.Id);
    }
}
