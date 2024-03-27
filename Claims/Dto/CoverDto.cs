using Claims.Domain;

namespace Claims.Dto;

public record CoverDto(
    string Id,
    DateOnly StartDate,
    DateOnly EndDate,
    CoverType Type,
    decimal Premium
);
