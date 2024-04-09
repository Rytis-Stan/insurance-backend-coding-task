namespace Claims.Api.Dto;

public record CoverDto(
    Guid Id,
    DateOnly StartDate,
    DateOnly EndDate,
    CoverDtoType Type,
    decimal Premium
);
