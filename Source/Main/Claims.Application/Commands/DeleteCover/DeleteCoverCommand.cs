using Claims.Application.Repositories;

namespace Claims.Application.Commands.DeleteCover;

public class DeleteCoverCommand : ICommandWithNoResults<DeleteCoverArgs>
{
    private readonly ICoversRepository _coversRepository;

    public DeleteCoverCommand(ICoversRepository coversRepository)
    {
        _coversRepository = coversRepository;
    }

    public async Task ExecuteAsync(DeleteCoverArgs args)
    {
        await _coversRepository.DeleteByIdAsync(args.Id);
    }
}
