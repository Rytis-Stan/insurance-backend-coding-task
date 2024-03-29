using Claims.Domain;
using Xunit;

namespace Claims.Tests;

// TODO: Rename the test class when the premium calculation logic gets placed in its proper place.
public class PremiumTests
{
    [Fact]
    public void PremiumForASingleDayIsEqualToBaseDayRateForTheSpecificCoverType()
    {
        Test(new DateOnly(2000, 01, 01), CoverType.Yacht, 1375);         // 10% of 1250
        Test(new DateOnly(2000, 01, 01), CoverType.PassengerShip, 1500); // 20% of 1250
        Test(new DateOnly(2000, 01, 01), CoverType.Tanker, 1875);        // 50% of 1250
        Test(new DateOnly(2000, 01, 01), CoverType.ContainerShip, 1625); // 30% of 1250
        Test(new DateOnly(2000, 01, 01), CoverType.BulkCarrier, 1625);   // 30% of 1250

        void Test(DateOnly startAndEndDate, CoverType coverType, decimal expectedPremium)
        {
            Assert.Equal(
                expectedPremium,
                Cover.ComputePremium(startAndEndDate, startAndEndDate, coverType)
            );
        }
    }
}
