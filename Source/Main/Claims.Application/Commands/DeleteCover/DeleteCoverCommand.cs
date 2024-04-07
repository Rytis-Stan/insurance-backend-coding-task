using Claims.Application.Repositories;

namespace Claims.Application.Commands.DeleteCover;

public class DeleteCoverCommand : ICommandWithNoResults<DeleteCoverRequest>
{
    private readonly ICoversRepository _coversRepository;

    public DeleteCoverCommand(ICoversRepository coversRepository)
    {
        _coversRepository = coversRepository;
    }

    public async Task ExecuteAsync(DeleteCoverRequest request)
    {
        await _coversRepository.DeleteByIdAsync(request.Id);
    }
}
