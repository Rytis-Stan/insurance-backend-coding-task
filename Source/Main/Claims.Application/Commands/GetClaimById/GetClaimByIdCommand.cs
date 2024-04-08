using Claims.Application.Repositories;

namespace Claims.Application.Commands.GetClaimById;

public class GetClaimByIdCommand : ICommand<GetClaimByIdArgs, GetClaimByIdResponse>
{
    private readonly IClaimsRepository _claimsRepository;

    public GetClaimByIdCommand(IClaimsRepository claimsRepository)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task<GetClaimByIdResponse> ExecuteAsync(GetClaimByIdArgs args)
    {
        var claim = await _claimsRepository.FindByIdAsync(args.Id);
        return new GetClaimByIdResponse(claim);
    }
}
