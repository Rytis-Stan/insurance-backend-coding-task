using Claims.Application.Repositories;

namespace Claims.Application.Commands;

public class GetCoverCommand : IGetCoverCommand
{
    private readonly ICoversRepository _coversRepository;

    public GetCoverCommand(ICoversRepository coversRepository)
    {
        _coversRepository = coversRepository;
    }

    public async Task<GetCoverResponse> ExecuteAsync(GetCoverRequest request)
    {
        var cover = await _coversRepository.FindByIdAsync(request.Id);
        return new GetCoverResponse(cover);
    }
}
