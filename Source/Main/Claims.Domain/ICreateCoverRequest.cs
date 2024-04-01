namespace Claims.Domain;

public interface ICreateCoverRequest
{
    DateOnly StartDate { get; }
    DateOnly EndDate { get; }
    CoverType Type { get; }
}

public record CreateCoverRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    CoverType Type
) : ICreateCoverRequest;