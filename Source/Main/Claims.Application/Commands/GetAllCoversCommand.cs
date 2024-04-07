using Claims.Application.Repositories;

namespace Claims.Application.Commands;

public class GetAllCoversCommand : INoParametersCommand<GetAllCoversResponse>
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
