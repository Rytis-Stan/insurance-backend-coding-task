using Claims.Domain;

namespace Claims.Application.Commands;

public record GetCoverPremiumRequest(DateOnly StartDate, DateOnly EndDate, CoverType CoverType);
