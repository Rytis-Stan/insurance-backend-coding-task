using Claims.Application.Repositories;

namespace Claims.Application.Commands;

public class GetAllClaimsCommand : IGetAllClaimsCommand
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
