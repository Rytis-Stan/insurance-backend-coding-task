using Claims.Application.Repositories;

namespace Claims.Application.Commands;

public class DeleteCoverCommand : INoResultsCommand<DeleteCoverRequest>
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
