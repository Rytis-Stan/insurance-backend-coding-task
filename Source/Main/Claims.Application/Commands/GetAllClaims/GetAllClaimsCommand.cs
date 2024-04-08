using Claims.Application.Repositories;

namespace Claims.Application.Commands.GetAllClaims;

public class GetAllClaimsCommand : ICommandWithNoArgs<GetAllClaimsResult>
{
    private readonly IClaimsRepository _claimsRepository;

    public GetAllClaimsCommand(IClaimsRepository claimsRepository)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task<GetAllClaimsResult> ExecuteAsync()
    {
        var claims = await _claimsRepository.GetAllAsync();
        return new GetAllClaimsResult(claims);
    }
}
