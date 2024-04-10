using Claims.Application.Repositories;

namespace Claims.Application.Commands.GetCovers;

public class GetCoversCommand : ICommandWithNoArgs<GetCoversResult>
{
    private readonly ICoversRepository _coversRepository;

    public GetCoversCommand(ICoversRepository coversRepository)
    {
        _coversRepository = coversRepository;
    }

    public async Task<GetCoversResult> ExecuteAsync()
    {
        var covers = await _coversRepository.GetAllAsync();
        return new GetCoversResult(covers);
    }
}
