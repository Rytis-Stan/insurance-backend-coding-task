using Claims.Application.Repositories;
using Claims.Domain;

namespace Claims.Application.Commands;

public class GetCoverCommand : IGetCoverCommand
{
    private readonly ICoversRepository _coversRepository;

    public GetCoverCommand(ICoversRepository coversRepository)
    {
        _coversRepository = coversRepository;
    }

    public async Task<Cover?> ExecuteAsync(GetCoverRequest request)
    {
        return await _coversRepository.FindByIdAsync(request.Id);
    }
}
