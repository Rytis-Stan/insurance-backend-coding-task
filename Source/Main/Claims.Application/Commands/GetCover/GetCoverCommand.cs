using Claims.Application.Repositories;

namespace Claims.Application.Commands.GetCover;

public class GetCoverCommand : ICommand<GetCoverArgs, GetCoverResponse>
{
    private readonly ICoversRepository _coversRepository;

    public GetCoverCommand(ICoversRepository coversRepository)
    {
        _coversRepository = coversRepository;
    }

    public async Task<GetCoverResponse> ExecuteAsync(GetCoverArgs args)
    {
        var cover = await _coversRepository.FindByIdAsync(args.Id);
        return new GetCoverResponse(cover);
    }
}
