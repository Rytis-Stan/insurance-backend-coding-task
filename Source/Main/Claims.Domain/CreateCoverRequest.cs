namespace Claims.Domain;

public record CreateCoverRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    CoverType Type
);
