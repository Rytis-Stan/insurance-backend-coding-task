namespace Claims.Domain.Repositories;

public record NewCoverInfo(
    DateOnly StartDate,
    DateOnly EndDate,
    CoverType Type,
    decimal Premium
) : INewCoverInfo;
