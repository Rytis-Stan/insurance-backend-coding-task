using Claims.Application.Repositories;

namespace Claims.Application.Commands.GetAllClaims;

public class GetAllClaimsCommand : ICommandWithNoParameters<GetAllClaimsResponse>
{
    private readonly IClaimsRepository _claimsRepository;

    public GetAllClaimsCommand(IClaimsRepository claimsRepository)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task<GetAllClaimsResponse> ExecuteAsync()
    {
        var claims = await _claimsRepository.GetAllAsync();
        return new GetAllClaimsResponse(claims);
    }
}
