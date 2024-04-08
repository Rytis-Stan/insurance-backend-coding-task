using Claims.Application.Repositories;

namespace Claims.Application.Commands.DeleteClaim;

public class DeleteClaimCommand : ICommandWithNoResults<DeleteClaimRequest>
{
    private readonly IClaimsRepository _claimsRepository;

    public DeleteClaimCommand(IClaimsRepository claimsRepository)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task ExecuteAsync(DeleteClaimRequest request)
    {
        await _claimsRepository.DeleteByIdAsync(request.Id);
    }
}