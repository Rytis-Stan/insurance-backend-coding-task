using Claims.Domain;

namespace Claims.Application.Commands;

public interface ICoverPricing
{
    decimal CalculatePremium(DateOnly startDate, DateOnly endDate, CoverType coverType);
}
