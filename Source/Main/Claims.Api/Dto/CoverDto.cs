namespace Claims.Api.Dto;

public record CoverDto(
    Guid Id,
    DateOnly StartDate,
    DateOnly EndDate,
    CoverTypeDto Type,
    decimal Premium
);
