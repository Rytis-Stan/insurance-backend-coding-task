namespace Claims.Api.Dto;

public record CreateCoverRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    CoverDtoType Type
);
