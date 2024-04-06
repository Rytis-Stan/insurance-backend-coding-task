namespace Claims.Api.Dto;

public record CreateCoverRequestDto(
    DateOnly StartDate,
    DateOnly EndDate,
    CoverTypeDto Type
);
