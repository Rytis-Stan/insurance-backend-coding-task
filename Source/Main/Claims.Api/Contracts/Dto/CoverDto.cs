namespace Claims.Api.Contracts.Dto;

public record CoverDto(
    Guid Id,
    DateOnly StartDate,
    DateOnly EndDate,
    CoverDtoType Type,
    decimal Premium
);
