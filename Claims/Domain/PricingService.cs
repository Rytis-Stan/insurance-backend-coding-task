namespace Claims.Domain;

public class PricingService : IPricingService
{
    public decimal CalculatePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var insurancePeriodInDays = endDate.DayNumber + 1 - startDate.DayNumber;

        var daysInInitial30DayPeriod = Math.Min(insurancePeriodInDays, 30);
        var daysInLater150DayPeriod = Math.Clamp(insurancePeriodInDays - 30, 0, 150);
        var daysInRemainingPeriod = Math.Max(insurancePeriodInDays - 30 - 150, 0);

        var premiumForInitial30Days = daysInInitial30DayPeriod * DayRateFor(coverType);
        var premiumForLater150Days = daysInLater150DayPeriod * DayRateFor(coverType) * DayBaseRateCoefficientForDay31To180(coverType);
        var premiumForRemainingDays = daysInRemainingPeriod * DayRateFor(coverType) * DayBaseRateCoefficientForDaysAfter180(coverType);
        var totalPremium = premiumForInitial30Days + premiumForLater150Days + premiumForRemainingDays;

        return totalPremium;
    }

    private decimal DayRateFor(CoverType coverType)
    {
        const decimal baseDayRate = 1250.00m;
        return baseDayRate * DayRateCoefficientFor(coverType);
    }

    private decimal DayRateCoefficientFor(CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => 1.1m,
            CoverType.PassengerShip => 1.2m,
            CoverType.Tanker => 1.5m,
            _ => 1.3m
        };
    }

    private decimal DayBaseRateCoefficientForDay31To180(CoverType coverType)
    {
        return coverType == CoverType.Yacht
            ? 1.00m - 0.05m
            : 1.00m - 0.02m;
    }

    private decimal DayBaseRateCoefficientForDaysAfter180(CoverType coverType)
    {
        return coverType == CoverType.Yacht
            ? 1.00m - 0.08m
            : 1.00m - 0.03m;
    }
}
