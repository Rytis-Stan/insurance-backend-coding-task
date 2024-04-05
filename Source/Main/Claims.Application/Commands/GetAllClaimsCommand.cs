using Claims.Application.Repositories;
using Claims.Domain;

namespace Claims.Application.Commands;

public class GetAllClaimsCommand : IGetAllClaimsCommand
{
    private readonly IClaimsRepository _claimsRepository;

    public GetAllClaimsCommand(IClaimsRepository claimsRepository)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task<IEnumerable<Claim>> ExecuteAsync()
    {
        return await _claimsRepository.GetAllAsync();
    }
}
