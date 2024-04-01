using Claims.Domain;

namespace Claims.Api.Dto;

public record CoverDto(
    Guid Id,
    DateOnly StartDate,
    DateOnly EndDate,
    CoverType Type,
    decimal Premium
);
