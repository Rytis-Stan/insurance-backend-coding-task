using Claims.Domain;

namespace Claims.Dto;

public class CreateCoverRequestDto : ICreateCoverRequest
{
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public CoverType Type { get; init; }
}
