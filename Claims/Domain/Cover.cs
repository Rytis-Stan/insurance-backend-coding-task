using System.Diagnostics;

namespace Claims.Domain;

public class Cover
{
    public required Guid Id { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public required CoverType Type { get; init; }
    public required decimal Premium { get; init; }

    // TODO: Make the "Premium" property call "ComputerPremium".
    public static decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var insuranceDurationInDays = endDate.DayNumber + 1 - startDate.DayNumber;
        var totalPremium = 0m;

        for (var day = 0; day < insuranceDurationInDays; day++)
        {
            totalPremium += PremiumPerDay(coverType, day);
        }

        return totalPremium;
    }

    private static decimal PremiumPerDay(CoverType coverType, int day)
    {
        var dayRate = DayRateFor(coverType);
        return dayRate * DayBasedRateCoefficientFor(day, coverType);
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

    private static decimal DayBasedRateCoefficientFor(int day, CoverType coverType)
    {
        if (day < 30)
        {
            return 1.00m;
        }

        if (day < 180)
        {
            return coverType == CoverType.Yacht
                ? 1.00m - 0.05m
                : 1.00m - 0.02m;
        }

        if (day < 365)
        {
            return coverType == CoverType.Yacht
                ? 1.00m - 0.08m
                : 1.00m - 0.03m;
        }

        // TODO: Add proper handling of this exception somewhere (need to fix the max period for the premium calculations).
        throw new UnreachableException("This code should not be reached.");
    }
}

public enum CoverType
{
    Yacht = 0,
    PassengerShip = 1,
    ContainerShip = 2,
    BulkCarrier = 3,
    Tanker = 4
}