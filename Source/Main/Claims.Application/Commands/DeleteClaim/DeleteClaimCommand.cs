using Claims.Application.Repositories;

namespace Claims.Application.Commands.DeleteClaim;

public class DeleteClaimCommand : ICommandWithNoResults<DeleteClaimArgs>
{
    private readonly IClaimsRepository _claimsRepository;

    public DeleteClaimCommand(IClaimsRepository claimsRepository)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task ExecuteAsync(DeleteClaimArgs args)
    {
        await _claimsRepository.DeleteByIdAsync(args.Id);
    }
}
