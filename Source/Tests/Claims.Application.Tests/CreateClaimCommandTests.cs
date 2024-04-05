using Claims.Application.Commands;
using Claims.Application.Repositories;
using Claims.Domain;
using Claims.Testing;
using Moq;
using Xunit;

namespace Claims.Application.Tests;

public class CreateClaimCommandTests : ClaimsCommandTests
{
    private const ClaimType AnyClaimType = ClaimType.BadWeather;
    private const decimal AnyDamageCost = 10.00m;
    private const CoverType AnyCoverType = CoverType.BulkCarrier;
    private const decimal AnyPremium = 100.00m;

    private readonly Mock<ICoversRepository> _coversRepositoryMock;
    private readonly ICreateClaimCommand _createClaimCommand;

    public CreateClaimCommandTests()
    {
        _coversRepositoryMock = new Mock<ICoversRepository>();
        _createClaimCommand = new CreateClaimCommand(_claimsRepositoryMock.Object, _coversRepositoryMock.Object);
    }

    [Theory]
    [InlineData(-2.00)]
    [InlineData(-1.00)]
    [InlineData(-0.01)]
    [InlineData(0.00)]
    public async Task ThrowsExceptionWhenCreatingAClaimWithNonPositiveDamageCost(decimal damageCost)
    {
        var coverId = Guid.NewGuid();
        var created = TestData.RandomUtcDateTime();
        var request = new CreateClaimRequest(coverId, "anyName", AnyClaimType, damageCost, created);

        await AssertExtended.ThrowsArgumentExceptionAsync(
            () => _createClaimCommand.CreateClaimAsync(request),
            "Damage cost must be a positive value."
        );
    }

    [Theory]
    [InlineData(100_001.00)]
    [InlineData(100_002.00)]
    [InlineData(100_003.00)]
    public async Task ThrowsExceptionWhenCreatingAClaimWithDamageCostExceedingMaxAllowedValueOfAHundredThousand(decimal damageCost)
    {
        var coverId = Guid.NewGuid();
        var created = TestData.RandomUtcDateTime();
        var request = new CreateClaimRequest(coverId, "anyName", AnyClaimType, damageCost, created);

        await AssertExtended.ThrowsArgumentExceptionAsync(
            () => _createClaimCommand.CreateClaimAsync(request),
            "Damage cost cannot exceed 100000."
        );
    }

    // TODO: Should the thrown exception be an ArgumentException?
    [Fact]
    public async Task ThrowsExceptionWhenCreatingAClaimWithNonExistingCoverId()
    {
        var coverId = Guid.NewGuid();
        var created = TestData.RandomUtcDateTime();
        var request = new CreateClaimRequest(coverId, "anyName", AnyClaimType, AnyDamageCost, created);
        StubFindCover(coverId, null);

        await AssertExtended.ThrowsArgumentExceptionAsync(
            () => _createClaimCommand.CreateClaimAsync(request),
            "Claim references a non-existing cover via the cover ID."
        );
    }

    [Fact]
    public async Task ThrowsExceptionWhenCreatingAClaimForADateOutsideOfRelatedCoverPeriod()
    {
        await Test(
            TestValueBuilder.Date(2000, 10, 20),
            TestValueBuilder.Date(2000, 10, 20),
            TestValueBuilder.UtcDateTime(2000, 10, 18)
        );
        await Test(
            TestValueBuilder.Date(2000, 10, 20),
            TestValueBuilder.Date(2000, 10, 20),
            TestValueBuilder.UtcDateTime(2000, 10, 19)
        );
        await Test(
            TestValueBuilder.Date(1987, 06, 05),
            TestValueBuilder.Date(1987, 06, 05),
            TestValueBuilder.UtcDateTime(1987, 06, 04)
        );
        await Test(
            TestValueBuilder.Date(1987, 06, 05),
            TestValueBuilder.Date(1987, 06, 05),
            TestValueBuilder.UtcDateTime(1987, 06, 06)
        );
        await Test(
            TestValueBuilder.Date(1987, 06, 05),
            TestValueBuilder.Date(1987, 06, 05),
            TestValueBuilder.UtcDateTime(1987, 06, 07)
        );
        await Test(
            TestValueBuilder.Date(1987, 06, 05),
            TestValueBuilder.Date(1987, 06, 06),
            TestValueBuilder.UtcDateTime(1987, 06, 07)
        );

        async Task Test(DateOnly coverStartDate, DateOnly coverEndDate, DateTime claimCreated)
        {
            var cover = CreateCoverForPeriod(coverStartDate, coverEndDate);
            var request = new CreateClaimRequest(cover.Id, "anyName", AnyClaimType, AnyDamageCost, claimCreated);
            StubFindCover(cover.Id, cover);

            await AssertExtended.ThrowsArgumentExceptionAsync(
                () => _createClaimCommand.CreateClaimAsync(request),
                "Claim is outside the related cover period."
            );
        }
    }

    [Fact]
    public async Task AddsClaimToRepositoryWhenCreatingAValidClaim()
    {
        await Test(TestValueBuilder.Date(2000, 10, 10), TestValueBuilder.Date(2000, 10, 15), "someClaim", ClaimType.BadWeather, 123.45m, TestValueBuilder.UtcDateTime(2000, 10, 10));
        await Test(TestValueBuilder.Date(2000, 10, 10), TestValueBuilder.Date(2000, 10, 15), "claimNo1", ClaimType.BadWeather, 123.45m, TestValueBuilder.UtcDateTime(2000, 10, 10));
        await Test(TestValueBuilder.Date(2000, 10, 10), TestValueBuilder.Date(2000, 10, 15), "claimNo1", ClaimType.Fire, 123.45m, TestValueBuilder.UtcDateTime(2000, 10, 10));
        await Test(TestValueBuilder.Date(2000, 10, 10), TestValueBuilder.Date(2000, 10, 15), "claimNo1", ClaimType.Fire, 67.89m, TestValueBuilder.UtcDateTime(2000, 10, 10));
        await Test(TestValueBuilder.Date(2000, 10, 10), TestValueBuilder.Date(2000, 10, 15), "claimNo1", ClaimType.Fire, 67.89m, TestValueBuilder.UtcDateTime(2000, 10, 11));
        await Test(TestValueBuilder.Date(2000, 10, 10), TestValueBuilder.Date(2000, 10, 15), "claimNo1", ClaimType.Fire, 100000.00m, TestValueBuilder.UtcDateTime(2000, 10, 11));
        await Test(TestValueBuilder.Date(2000, 10, 10), TestValueBuilder.Date(2000, 10, 15), "claimNo1", ClaimType.Fire, 100000.00m, TestValueBuilder.UtcDateTime(2000, 10, 14));
        await Test(TestValueBuilder.Date(2000, 10, 10), TestValueBuilder.Date(2000, 10, 15), "claimNo1", ClaimType.Fire, 100000.00m, TestValueBuilder.UtcDateTime(2000, 10, 15));
        await Test(TestValueBuilder.Date(1996, 03, 04), TestValueBuilder.Date(1996, 05, 06), "claimB", ClaimType.Collision, 0.01m, TestValueBuilder.UtcDateTime(1996, 03, 04));

        async Task Test(DateOnly coverStartDate, DateOnly coverEndDate, string claimName, ClaimType claimType, decimal claimDamageCost, DateTime claimCreated)
        {
            var cover = CreateCoverForPeriod(coverStartDate, coverEndDate);
            var request = new CreateClaimRequest(cover.Id, claimName, claimType, claimDamageCost, claimCreated);
            StubFindCover(cover.Id, cover);

            await _createClaimCommand.CreateClaimAsync(request);

            _claimsRepositoryMock.Verify(x => x.CreateAsync(new NewClaimInfo(cover.Id, claimName, claimType, claimDamageCost, claimCreated)));
        }
    }

    private static Cover CreateCoverForPeriod(DateOnly startDate, DateOnly endDate)
    {
        return new Cover
        {
            Id = Guid.NewGuid(),
            StartDate = startDate,
            EndDate = endDate,
            Type = AnyCoverType,
            Premium = AnyPremium
        };
    }

    private void StubFindCover(Guid id, Cover? coverToReturn)
    {
        _coversRepositoryMock
            .Setup(x => x.FindByIdAsync(id))
            .ReturnsAsync(coverToReturn);
    }
}