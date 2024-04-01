namespace Claims.Repositories;

public record NewCoverInfo(
    DateOnly StartDate,
    DateOnly EndDate,
    CoverType Type,
    decimal Premium
) : INewCoverInfo;
