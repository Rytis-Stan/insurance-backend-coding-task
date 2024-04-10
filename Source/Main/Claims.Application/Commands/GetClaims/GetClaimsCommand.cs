using Claims.Application.Repositories;

namespace Claims.Application.Commands.GetClaims;

public class GetClaimsCommand : ICommandWithNoArgs<GetClaimsResult>
{
    private readonly IClaimsRepository _claimsRepository;

    public GetClaimsCommand(IClaimsRepository claimsRepository)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task<GetClaimsResult> ExecuteAsync()
    {
        var claims = await _claimsRepository.GetAllAsync();
        return new GetClaimsResult(claims);
    }
}
