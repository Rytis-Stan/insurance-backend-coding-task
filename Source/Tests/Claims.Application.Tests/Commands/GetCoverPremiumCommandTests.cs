using Claims.Application.Commands.GetCoverPremium;
using Claims.Domain;
using Moq;
using Xunit;
using static BuildingBlocks.Testing.TestData;

namespace Claims.Application.Tests.Commands;

public class GetCoverPremiumCommandTests
{
    private readonly Mock<ICoverPricing> _coverPricingMock;
    private readonly GetCoverPremiumCommand _command;

    public GetCoverPremiumCommandTests()
    {
        _coverPricingMock = new Mock<ICoverPricing>();
        _command = new GetCoverPremiumCommand(_coverPricingMock.Object);
    }

    [Theory]
    [InlineData(12.34)]
    [InlineData(456.78)]
    public async Task ReturnsPremiumFromCoverPricing(decimal expectedPremium)
    {
        var startDate = RandomDate();
        var endDate = RandomDate();
        var coverType = RandomEnum<CoverType>();
        StubPremium(startDate, endDate, coverType, expectedPremium);

        var result = await _command.ExecuteAsync(new GetCoverPremiumArgs(startDate, endDate, coverType));

        Assert.Equal(expectedPremium, result.Premium);
    }

    private void StubPremium(DateOnly startDate, DateOnly endDate, CoverType coverType, decimal premium)
    {
        _coverPricingMock
            .Setup(x => x.Premium(startDate, endDate, coverType))
            .Returns(premium);
    }
}
