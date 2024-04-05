using Claims.Application.Repositories;
using Claims.Domain;

namespace Claims.Application.Commands;

public class GetAllCoversCommand : IGetAllCoversCommand
{
    private readonly ICoversRepository _coversRepository;

    public GetAllCoversCommand(ICoversRepository coversRepository)
    {
        _coversRepository = coversRepository;
    }

    public async Task<IEnumerable<Cover>> ExecuteAsync()
    {
        return await _coversRepository.GetAllAsync();
    }
}
