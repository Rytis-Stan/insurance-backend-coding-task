using Claims.Domain;

namespace Claims.Application.Repositories;

public record NewCoverInfo(
    DateOnly StartDate,
    DateOnly EndDate,
    CoverType Type,
    decimal Premium
);
