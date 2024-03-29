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

    private DateOnly RandomDate()
    {
        return new DateOnly(2000, 01, 01).AddDays(Random.Next(10_000));
    }
}
