using Claims.Domain;

namespace Claims.Dto;

public record CreateCoverRequestDto(
    DateOnly StartDate,
    DateOnly EndDate,
    CoverType Type
) : ICreateCoverRequest;
