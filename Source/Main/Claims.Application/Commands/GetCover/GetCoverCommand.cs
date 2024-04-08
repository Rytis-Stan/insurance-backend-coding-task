using Claims.Application.Repositories;

namespace Claims.Application.Commands.GetCover;

public class GetCoverCommand : ICommand<GetCoverArgs, GetCoverResult>
{
    private readonly ICoversRepository _coversRepository;

    public GetCoverCommand(ICoversRepository coversRepository)
    {
        _coversRepository = coversRepository;
    }

    public async Task<GetCoverResult> ExecuteAsync(GetCoverArgs args)
    {
        var cover = await _coversRepository.FindByIdAsync(args.Id);
        return new GetCoverResult(cover);
    }
}
