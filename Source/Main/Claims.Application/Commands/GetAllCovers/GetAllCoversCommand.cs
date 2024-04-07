using Claims.Application.Repositories;

namespace Claims.Application.Commands.GetAllCovers;

public class GetAllCoversCommand : ICommandWithNoParameters<GetAllCoversResponse>
{
    private readonly ICoversRepository _coversRepository;

    public GetAllCoversCommand(ICoversRepository coversRepository)
    {
        _coversRepository = coversRepository;
    }

    public async Task<GetAllCoversResponse> ExecuteAsync()
    {
        var covers = await _coversRepository.GetAllAsync();
        return new GetAllCoversResponse(covers);
    }
}
