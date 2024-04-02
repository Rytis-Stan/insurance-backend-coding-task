using Claims.Domain;

namespace Claims.Application;

public record CreateCoverRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    CoverType Type
);
