using Claims.Domain;
using Xunit;

namespace Claims.Tests;

// TODO: Rename the test class when the premium calculation logic gets placed in its proper place.
public class PremiumTests
{
    private static readonly Random Random = new Random();

    [Theory]
    [InlineData(CoverType.Yacht, 1375)]         // 10% of 1250
    [InlineData(CoverType.PassengerShip, 1500)] // 20% of 1250
    [InlineData(CoverType.Tanker, 1875)]        // 50% of 1250
    [InlineData(CoverType.ContainerShip, 1625)] // 30% of 1250
    [InlineData(CoverType.BulkCarrier, 1625)]   // 30% of 1250
    public void PremiumForASingleDayIsEqualToBaseDayRateForTheSpecificCoverType(CoverType coverType, decimal expectedPremium)
    {
        var startAndEndDate = RandomDate();
        Assert.Equal(
            expectedPremium,
            Cover.ComputePremium(startAndEndDate, startAndEndDate, coverType)
        );
    }

    [Theory]
    [InlineData(CoverType.Yacht, 2, 2750)]
    [InlineData(CoverType.Yacht, 3, 4125)]
    [InlineData(CoverType.Yacht, 29, 39875)]
    [InlineData(CoverType.Yacht, 30, 41250)]

    [InlineData(CoverType.PassengerShip, 2, 3000)]
    [InlineData(CoverType.PassengerShip, 3, 4500)]
    [InlineData(CoverType.PassengerShip, 29, 43500)]
    [InlineData(CoverType.PassengerShip, 30, 45000)]

    [InlineData(CoverType.Tanker, 2, 3750)]
    [InlineData(CoverType.Tanker, 3, 5625)]
    [InlineData(CoverType.Tanker, 29, 54375)]
    [InlineData(CoverType.Tanker, 30, 56250)]

    [InlineData(CoverType.ContainerShip, 2, 3250)]
    [InlineData(CoverType.ContainerShip, 3, 4875)]
    [InlineData(CoverType.ContainerShip, 29, 47125)]
    [InlineData(CoverType.ContainerShip, 30, 48750)]

    [InlineData(CoverType.BulkCarrier, 2, 3250)]
    [InlineData(CoverType.BulkCarrier, 3, 4875)]
    [InlineData(CoverType.BulkCarrier, 29, 47125)]
    [InlineData(CoverType.BulkCarrier, 30, 48750)]
    public void PremiumForFirst30DaysIsEqualToBaseDayRateForTheSpecificCoverTypeMultipliedByNumberOfPeriodDays(
        CoverType coverType, int periodDurationInDays, decimal expectedPremium)
    {
        var (startDate, endDate) = RandomPeriod(periodDurationInDays);
        Assert.Equal(
            expectedPremium,
            Cover.ComputePremium(startDate, endDate, coverType)
        );
    }

    [Theory]
    [InlineData(CoverType.Yacht, 31, 42556.25)] // Base Yacht's premium for days for the next 150 days is 95% of 1250 = 1306.25
    [InlineData(CoverType.Yacht, 32, 43862.50)]
    [InlineData(CoverType.Yacht, 179, 235881.25)]
    [InlineData(CoverType.Yacht, 180, 237187.50)]
    public void PremiumForNext150DaysAfterThe30ThIsEqualTo30DayPremiumPlusTheExtraDayBaseRateCalculatedWithADiscount(
        CoverType coverType, int periodDurationInDays, decimal expectedPremium)
    {
        var (startDate, endDate) = RandomPeriod(periodDurationInDays);
        Assert.Equal(
            expectedPremium,
            Cover.ComputePremium(startDate, endDate, coverType)
        );
    }

    private DateOnly RandomDate()
    {
        return new DateOnly(2000, 01, 01).AddDays(Random.Next(10_000));
    }

    private (DateOnly Start, DateOnly End) RandomPeriod(int durationInDays)
    {
        var startDate = RandomDate();
        var endDate = startDate.AddDays(durationInDays - 1);
        return (startDate, endDate);
    }
}
