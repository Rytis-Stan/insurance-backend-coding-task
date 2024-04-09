using Claims.Api.Contracts.Dto;

namespace Claims.Api.Contracts.Messages;

public record CreateCoverRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    CoverDtoType Type
);
