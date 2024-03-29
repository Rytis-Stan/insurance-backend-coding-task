using Claims.Domain;
using Xunit;

namespace Claims.Tests;

// TODO: Rename the test class when the premium calculation logic gets placed in its proper place.
public class PremiumTests
{
    [Theory]
    [InlineData(2000, 01, 01, CoverType.Yacht, 1375)]         // 10% of 1250
    [InlineData(2000, 01, 01, CoverType.PassengerShip, 1500)] // 20% of 1250
    [InlineData(2000, 01, 01, CoverType.Tanker, 1875)]        // 50% of 1250
    [InlineData(2000, 01, 01, CoverType.ContainerShip, 1625)] // 30% of 1250
    [InlineData(2000, 01, 01, CoverType.BulkCarrier, 1625)]   // 30% of 1250
    public void PremiumForASingleDayIsEqualToBaseDayRateForTheSpecificCoverType(int year, int month, int day, CoverType coverType, decimal expectedPremium)
    {
        var startAndEndDate = new DateOnly(year, month, day);
        Assert.Equal(
            expectedPremium,
            Cover.ComputePremium(startAndEndDate, startAndEndDate, coverType)
        );
    }
}
