namespace Claims.Domain;

public class CoverPricing : ICoverPricing
{
    public decimal Premium(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        ValidatePeriodNotTooLong(startDate, endDate);
        return PremiumFor(startDate, endDate, coverType);
    }

    private static void ValidatePeriodNotTooLong(DateOnly startDate, DateOnly endDate)
    {
        if (startDate.AddYears(1) <= endDate)
        {
            throw new ValidationException("Total insurance period cannot exceed 1 year.");
        }
    }

    private static decimal PremiumFor(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var totalInsurancePeriodInDays = endDate.DayNumber - startDate.DayNumber + 1;

        const int initialPeriodDurationInDays = 30;
        const int laterPeriodDurationInDays = 150;

        var daysInInitialPeriod = Math.Min(totalInsurancePeriodInDays, initialPeriodDurationInDays);
        var daysInLaterPeriod = Math.Clamp(totalInsurancePeriodInDays - initialPeriodDurationInDays, 0, laterPeriodDurationInDays);
        var daysInRemainingPeriod = Math.Max(totalInsurancePeriodInDays - initialPeriodDurationInDays - laterPeriodDurationInDays, 0);

        var initialPeriodPremium = daysInInitialPeriod * DayRateFor(coverType);
        var laterPeriodPremium = daysInLaterPeriod * DayRateFor(coverType) * DayBaseRateCoefficientForLaterPeriod(coverType);
        var remainingPeriodPremium = daysInRemainingPeriod * DayRateFor(coverType) * DayBaseRateCoefficientForRemainingPeriod(coverType);
        var totalPremium = initialPeriodPremium + laterPeriodPremium + remainingPeriodPremium;

        return totalPremium;
    }

    private static decimal DayRateFor(CoverType coverType)
    {
        const decimal baseDayRate = 1250.00m;
        return baseDayRate * DayRateCoefficientFor(coverType);
    }

    private static decimal DayRateCoefficientFor(CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => 1.1m,
            CoverType.PassengerShip => 1.2m,
            CoverType.Tanker => 1.5m,
            _ => 1.3m
        };
    }

    private static decimal DayBaseRateCoefficientForLaterPeriod(CoverType coverType)
    {
        var discount = coverType == CoverType.Yacht
            ? 0.05m
            : 0.02m;
        return 1.00m - discount;
    }

    private static decimal DayBaseRateCoefficientForRemainingPeriod(CoverType coverType)
    {
        var discount = coverType == CoverType.Yacht
            ? 0.08m
            : 0.03m;
        return 1.00m - discount;
    }
}
