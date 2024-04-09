using Claims.Testing;
using Xunit;
using static Claims.Testing.TestValueBuilder;

namespace Claims.Domain.Tests;

public class CoverPricingTests
{
    private const CoverType AnyCoverType = CoverType.Tanker;

    private readonly CoverPricing _coverPricing;

    public CoverPricingTests()
    {
        _coverPricing = new CoverPricing();
    }

    [Fact]
    public void ThrowsExceptionWhenEndDateGoesBeforeStartDate()
    {
        Test(
            Date(2010, 01, 10),
            Date(2010, 01, 08)
        );
        Test(
            Date(2010, 01, 10),
            Date(2010, 01, 09)
        );
        Test(
            Date(2010, 01, 10),
            Date(2010, 01, 09)
        );
        Test(
            Date(1981, 06, 10),
            Date(1981, 06, 09)
        );
        Test(
            Date(1981, 06, 11),
            Date(1981, 06, 10)
        );

        void Test(DateOnly startDate, DateOnly endDate)
        {
            AssertExtended.Throws<ValidationException>(
                () => _coverPricing.Premium(startDate, endDate, AnyCoverType),
                "End date cannot be earlier than the start date."
            );
        }
    }

    [Fact]
    public void ThrowsExceptionWhenCoverPeriodExceedsASingleYear()
    {
        Test(
            Date(2000, 01, 01),
            Date(2001, 01, 01)
        );
        Test(
            Date(1972, 01, 01),
            Date(1973, 01, 01)
        );
        Test(
            Date(1972, 01, 01),
            Date(1999, 01, 01)
        );
        Test(
            Date(1972, 01, 01),
            Date(1999, 02, 03)
        );
        Test(
            Date(1998, 02, 03),
            Date(1999, 02, 03)
        );

        void Test(DateOnly startDate, DateOnly endDate)
        {
            AssertExtended.Throws<ValidationException>(
                () => _coverPricing.Premium(startDate, endDate, AnyCoverType),
                "Total insurance period cannot exceed 1 year."
            );
        }
    }

    [Theory]
    [InlineData(CoverType.Yacht, 1375)]         // 10% of 1250
    [InlineData(CoverType.PassengerShip, 1500)] // 20% of 1250
    [InlineData(CoverType.Tanker, 1875)]        // 50% of 1250
    [InlineData(CoverType.ContainerShip, 1625)] // 30% of 1250
    [InlineData(CoverType.BulkCarrier, 1625)]   // 30% of 1250
    public void PremiumForASingleDayIsEqualToBaseDayRateForTheSpecificCoverType(CoverType coverType, decimal expectedPremium)
    {
        AssertReturnsCorrectPremium(coverType, 1, expectedPremium);
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
        AssertReturnsCorrectPremium(coverType, periodDurationInDays, expectedPremium);
    }

    [Theory]
    [InlineData(CoverType.Yacht, 31, 42556.25)] // Base Yacht's premium for days between 31st to 180th (inclusively) is 95% of 1375 = 1306.25
    [InlineData(CoverType.Yacht, 32, 43862.50)]
    [InlineData(CoverType.Yacht, 179, 235881.25)]
    [InlineData(CoverType.Yacht, 180, 237187.50)]

    [InlineData(CoverType.PassengerShip, 31, 46470)] // Base Passenger Ship's premium for days between 31st to 180th (inclusively) is 98% of 1500 = 1470
    [InlineData(CoverType.PassengerShip, 32, 47940)]
    [InlineData(CoverType.PassengerShip, 179, 264030)]
    [InlineData(CoverType.PassengerShip, 180, 265500)]

    [InlineData(CoverType.Tanker, 31, 58087.50)] // Base Tanker's premium for days between 31st to 180th (inclusively) is 98% of 1875 = 1837.50
    [InlineData(CoverType.Tanker, 32, 59925.00)]
    [InlineData(CoverType.Tanker, 179, 330037.50)]
    [InlineData(CoverType.Tanker, 180, 331875.00)]

    [InlineData(CoverType.ContainerShip, 31, 50342.50)] // Base Container Ship's premium for days between 31st to 180th (inclusively) is 98% of 1625 = 1592.50
    [InlineData(CoverType.ContainerShip, 32, 51935.00)]
    [InlineData(CoverType.ContainerShip, 179, 286032.50)]
    [InlineData(CoverType.ContainerShip, 180, 287625.00)]

    [InlineData(CoverType.BulkCarrier, 31, 50342.50)] // Base Bulk Carrier's premium for days between 31st to 180th (inclusively) is 98% of 1625 = 1592.50
    [InlineData(CoverType.BulkCarrier, 32, 51935.00)]
    [InlineData(CoverType.BulkCarrier, 179, 286032.50)]
    [InlineData(CoverType.BulkCarrier, 180, 287625.00)]
    public void PremiumForNext150DaysAfterThe30ThIsEqualTo30DayPremiumPlusTheExtraDayBaseRateCalculatedWithADiscount(
        CoverType coverType, int periodDurationInDays, decimal expectedPremium)
    {
        AssertReturnsCorrectPremium(coverType, periodDurationInDays, expectedPremium);
    }

    [Theory]
    [InlineData(CoverType.Yacht, 181, 238452.50)] // Base Yacht's premium starting from the 181st (inclusively) is 92% of 1375 = 1265
    [InlineData(CoverType.Yacht, 182, 239717.50)]
    [InlineData(CoverType.Yacht, 183, 240982.50)]

    [InlineData(CoverType.PassengerShip, 181, 266955)] // Base Passenger Ship's premium starting from the 181st (inclusively) is 97% of 1500 = 1455
    [InlineData(CoverType.PassengerShip, 182, 268410)]
    [InlineData(CoverType.PassengerShip, 183, 269865)]

    [InlineData(CoverType.Tanker, 181, 333693.75)] // Base Tanker's premium starting from the 181st (inclusively) is 97% of 1875 = 1818.75
    [InlineData(CoverType.Tanker, 182, 335512.50)]
    [InlineData(CoverType.Tanker, 183, 337331.25)]

    [InlineData(CoverType.ContainerShip, 181, 289201.25)] // Base Container Ship's premium starting from the 181st (inclusively) is 97% of 1625 = 1576.25
    [InlineData(CoverType.ContainerShip, 182, 290777.50)]
    [InlineData(CoverType.ContainerShip, 183, 292353.75)]

    [InlineData(CoverType.BulkCarrier, 181, 289201.25)] // Base Bulk Carrier's premium starting from the 181st (inclusively) is 97% of 1625 = 1576.25
    [InlineData(CoverType.BulkCarrier, 182, 290777.5)]
    [InlineData(CoverType.BulkCarrier, 183, 292353.75)]
    public void PremiumForAllDaysAfterThe180ThIsEqualTo180DayPremiumPlusTheExtraDayBaseRateCalculatedWithADiscount(
        CoverType coverType, int periodDurationInDays, decimal expectedPremium)
    {
        AssertReturnsCorrectPremium(coverType, periodDurationInDays, expectedPremium);
    }

    private void AssertReturnsCorrectPremium(CoverType coverType, int periodDurationInDays, decimal expectedPremium)
    {
        var (startDate, endDate) = TestData.RandomFixedLengthPeriod(periodDurationInDays);
        Assert.Equal(
            expectedPremium,
            _coverPricing.Premium(startDate, endDate, coverType)
        );
    }
}
