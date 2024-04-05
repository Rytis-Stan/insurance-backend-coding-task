using Claims.Application.Repositories;
using Claims.Domain;

namespace Claims.Application.Commands;

public class DeleteClaimCommand : IDeleteClaimCommand
{
    private readonly IClaimsRepository _claimsRepository;

    public DeleteClaimCommand(IClaimsRepository claimsRepository)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task<Claim?> ExecuteAsync(Guid id)
    {
        return await _claimsRepository.DeleteByIdAsync(id);
    }
}