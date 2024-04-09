using BuildingBlocks.Temporal;
using Claims.Application.Commands.CreateCover;
using Claims.Application.Repositories;
using Claims.Domain;
using Claims.Testing;
using Moq;
using Xunit;
using static Claims.Testing.TestValueBuilder;

namespace Claims.Application.Tests.Commands;

public class CreateCoverCommandTests : CoversCommandTests
{
    private const CoverType AnyCoverType = CoverType.PassengerShip;

    private readonly Mock<ICoverPricing> _coverPricingMock;
    private readonly Mock<IClock> _clockMock;
    private readonly CreateCoverCommand _command;

    public CreateCoverCommandTests()
    {
        _coverPricingMock = new Mock<ICoverPricing>();
        _clockMock = new Mock<IClock>();
        _command = new CreateCoverCommand(CoversRepositoryMock.Object, _coverPricingMock.Object, _clockMock.Object);
    }

    [Fact]
    public async Task ThrowsExceptionWhenCreatingACoverWithStartDateThatIsInThePast()
    {
        await Test(
            UtcDateTime(2000, 01, 10),
            Date(2000, 01, 08)
        );
        await Test(
            UtcDateTime(2000, 01, 10),
            Date(2000, 01, 09)
        );
        await Test(
            UtcDateTime(1998, 07, 06),
            Date(1998, 07, 05)
        );
        await Test(
            UtcDateTime(1998, 07, 06),
            Date(1996, 05, 04)
        );

        async Task Test(DateTime utcNow, DateOnly startDate)
        {
            var endDate = DateOnly.FromDateTime(utcNow);
            var args = new CreateCoverArgs(startDate, endDate, AnyCoverType);
            StubUtcNow(utcNow);

            await AssertExtended.ThrowsAsync<ValidationException>(
                () => _command.ExecuteAsync(args),
                "Start date cannot be in the past."
            );
        }
    }

    [Fact]
    public async Task ThrowsExceptionWhenCreatingACoverWithEndDateThatIsInThePast()
    {
        await Test(
            UtcDateTime(2000, 01, 10),
            Date(2000, 01, 08)
        );
        await Test(
            UtcDateTime(2000, 01, 10),
            Date(2000, 01, 09)
        );
        await Test(
            UtcDateTime(1998, 07, 06),
            Date(1998, 07, 05)
        );
        await Test(
            UtcDateTime(1998, 07, 06),
            Date(1996, 05, 04)
        );

        async Task Test(DateTime utcNow, DateOnly endDate)
        {
            var startDate = DateOnly.FromDateTime(utcNow);
            var args = new CreateCoverArgs(startDate, endDate, AnyCoverType);
            StubUtcNow(utcNow);

            await AssertExtended.ThrowsAsync<ValidationException>(
                () => _command.ExecuteAsync(args),
                "End date cannot be in the past."
            );
        }
    }

    [Fact]
    public async Task AddsCoverToRepositoryWhenCreatingAValidCover()
    {
        await Test(UtcDateTime(2000, 10, 10), Date(2000, 10, 10), Date(2000, 10, 20), CoverType.BulkCarrier, 123.45m);
        await Test(UtcDateTime(2000, 10, 10), Date(2000, 10, 11), Date(2000, 10, 20), CoverType.BulkCarrier, 123.45m);
        await Test(UtcDateTime(2000, 10, 10), Date(2000, 10, 11), Date(2000, 10, 19), CoverType.BulkCarrier, 123.45m);
        await Test(UtcDateTime(2000, 10, 10), Date(2000, 10, 11), Date(2000, 10, 19), CoverType.Tanker, 123.45m);
        await Test(UtcDateTime(2000, 10, 10), Date(2000, 10, 11), Date(2000, 10, 19), CoverType.Tanker, 98.76m);
        await Test(UtcDateTime(1971, 03, 04), Date(1971, 03, 04), Date(1971, 05, 06), CoverType.Yacht, 101.00m);
        await Test(UtcDateTime(1971, 03, 04), Date(1971, 03, 04), Date(1972, 03, 03), CoverType.Yacht, 101.00m);

        async Task Test(DateTime utcNow, DateOnly startDate, DateOnly endDate, CoverType coverType, decimal premium)
        {
            var args = new CreateCoverArgs(startDate, endDate, coverType);
            StubUtcNow(utcNow);
            StubPremium(startDate, endDate, coverType, premium);

            await _command.ExecuteAsync(args);

            CoversRepositoryMock.Verify(x => x.CreateAsync(new NewCoverInfo(startDate, endDate, coverType, premium)));
        }
    }

    [Fact]
    public async Task ReturnsCreatedCoverWhenCreatingAValidCover()
    {
        var cover = TestDomainData.RandomCover();
        var utcNow = UtcDateTime(cover.StartDate);
        var startDate = cover.StartDate;
        var endDate = cover.EndDate;
        var coverType = cover.Type;
        var premium = cover.Premium;
        var args = new CreateCoverArgs(startDate, endDate, coverType);
        StubUtcNow(utcNow);
        StubPremium(startDate, endDate, coverType, premium);
        StubCreateCover(startDate, endDate, coverType, premium, cover);

        var result = await _command.ExecuteAsync(args);

        Assert.Equal(cover, result.Cover);
    }

    private void StubUtcNow(DateTime utcNow)
    {
        _clockMock
            .Setup(x => x.UtcNow())
            .Returns(utcNow);
    }

    private void StubPremium(DateOnly startDate, DateOnly endDate, CoverType coverType, decimal premium)
    {
        _coverPricingMock
            .Setup(x => x.Premium(startDate, endDate, coverType))
            .Returns(premium);
    }

    private void StubCreateCover(DateOnly startDate, DateOnly endDate, CoverType coverType, decimal premium, Cover coverToReturn)
    {
        CoversRepositoryMock
            .Setup(x => x.CreateAsync(new NewCoverInfo(startDate, endDate, coverType, premium)))
            .ReturnsAsync(coverToReturn);
    }
}
