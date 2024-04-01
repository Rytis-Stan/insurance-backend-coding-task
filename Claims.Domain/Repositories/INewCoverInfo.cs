namespace Claims.Domain.Repositories;

public interface INewCoverInfo
{
    DateOnly StartDate { get; }
    DateOnly EndDate { get; }
    CoverType Type { get; }
    decimal Premium { get; }
}
