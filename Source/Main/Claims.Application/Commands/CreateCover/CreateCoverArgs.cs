using Claims.Domain;

namespace Claims.Application.Commands.CreateCover;

public record CreateCoverArgs(DateOnly StartDate, DateOnly EndDate, CoverType Type);
