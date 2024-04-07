using Claims.Application.Repositories;

namespace Claims.Application.Commands;

public class DeleteClaimCommand : IDeleteClaimCommand
{
    private readonly IClaimsRepository _claimsRepository;

    public DeleteClaimCommand(IClaimsRepository claimsRepository)
    {
        _claimsRepository = claimsRepository;
    }

    public async Task ExecuteAsync(DeleteClaimRequest request)
    {
        await _claimsRepository.DeleteByIdAsync(request.Id);
    }
}
