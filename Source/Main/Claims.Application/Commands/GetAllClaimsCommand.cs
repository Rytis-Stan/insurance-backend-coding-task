using Claims.Application.Repositories;

namespace Claims.Application.Commands;

public class GetAllClaimsCommand : INoParametersCommand<GetAllClaimsResponse>
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
