using Claims.Domain;

namespace Claims.Application;

public interface IPricingService
{
    decimal CalculatePremium(DateOnly startDate, DateOnly endDate, CoverType coverType);
}
