using Claims.Domain;

namespace Claims.Application.Commands;

public interface IPricingService
{
    decimal CalculatePremium(DateOnly startDate, DateOnly endDate, CoverType coverType);
}
