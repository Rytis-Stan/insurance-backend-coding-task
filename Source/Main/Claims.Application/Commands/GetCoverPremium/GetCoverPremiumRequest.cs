using Claims.Domain;

namespace Claims.Application.Commands.GetCoverPremium;

public record GetCoverPremiumRequest(DateOnly StartDate, DateOnly EndDate, CoverType CoverType);
