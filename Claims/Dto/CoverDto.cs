using Claims.Domain;

namespace Claims.Dto;

public class CoverDto
{
    public string Id { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public CoverType Type { get; init; }
    public decimal Premium { get; init; }
}
