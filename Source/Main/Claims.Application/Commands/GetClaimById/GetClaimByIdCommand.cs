using Claims.Application.Repositories;

namespace Claims.Application.Commands.GetClaimById;

public class GetClaimByIdCommand : ICommand<GetClaimByIdRequest, GetClaimByIdResponse>
{
    private readonly IClaimsRepository _claimsRepository;

    public GetClaimByIdCommand(IClaimsRepository claimsRepository)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task<GetClaimByIdResponse> ExecuteAsync(GetClaimByIdRequest request)
    {
        var claim = await _claimsRepository.FindByIdAsync(request.Id);
        return new GetClaimByIdResponse(claim);
    }
}
