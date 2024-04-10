using Claims.Application.Repositories;

namespace Claims.Application.Commands.GetClaim;

public class GetClaimCommand : ICommand<GetClaimArgs, GetClaimResult>
{
    private readonly IClaimsRepository _claimsRepository;

    public GetClaimCommand(IClaimsRepository claimsRepository)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task<GetClaimResult> ExecuteAsync(GetClaimArgs args)
    {
        var claim = await _claimsRepository.FindByIdAsync(args.Id);
        return new GetClaimResult(claim);
    }
}
