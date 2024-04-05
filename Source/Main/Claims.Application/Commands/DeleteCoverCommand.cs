using Claims.Application.Repositories;
using Claims.Domain;

namespace Claims.Application.Commands;

public class DeleteCoverCommand : IDeleteCoverCommand
{
    private readonly ICoversRepository _coversRepository;

    public DeleteCoverCommand(ICoversRepository coversRepository)
    {
        _coversRepository = coversRepository;
    }

    public async Task<Cover?> ExecuteAsync(DeleteCoverRequest request)
    {
        return await _coversRepository.DeleteByIdAsync(request.Id);
    }
}
