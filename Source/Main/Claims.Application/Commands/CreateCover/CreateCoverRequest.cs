using Claims.Domain;

namespace Claims.Application.Commands.CreateCover;

public record CreateCoverRequest(DateOnly StartDate, DateOnly EndDate, CoverType Type);
