namespace Claims.Domain;

public interface INewCoverInfo
{
    DateOnly StartDate { get; }
    DateOnly EndDate { get; }
    CoverType Type { get; }
    decimal Premium { get; }
}
