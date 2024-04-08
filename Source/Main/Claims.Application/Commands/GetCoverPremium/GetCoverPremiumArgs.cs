using Claims.Domain;

namespace Claims.Application.Commands.GetCoverPremium;

public record GetCoverPremiumArgs(DateOnly StartDate, DateOnly EndDate, CoverType CoverType);
