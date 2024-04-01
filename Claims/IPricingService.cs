namespace Claims;

public interface IPricingService
{
    decimal CalculatePremium(DateOnly startDate, DateOnly endDate, CoverType coverType);
}
