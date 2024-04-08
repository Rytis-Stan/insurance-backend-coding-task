using Claims.Application.Repositories;

namespace Claims.Application.Commands.GetClaimById;

public class GetClaimByIdCommand : ICommand<GetClaimByIdArgs, GetClaimByIdResult>
{
    private readonly IClaimsRepository _claimsRepository;

    public GetClaimByIdCommand(IClaimsRepository claimsRepository)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task<GetClaimByIdResult> ExecuteAsync(GetClaimByIdArgs args)
    {
        var claim = await _claimsRepository.FindByIdAsync(args.Id);
        return new GetClaimByIdResult(claim);
    }
}
