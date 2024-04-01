﻿using Claims.Domain;
using Claims.Infrastructure;
using Claims.Testing;
using Moq;
using Xunit;
using static Claims.Testing.TestValueBuilder;

namespace Claims.Tests;

public class CoversServiceTests
{
    private const CoverType AnyCoverType = CoverType.PassengerShip;

    private readonly Mock<ICoversRepository> _coversRepositoryMock;
    private readonly Mock<IPricingService> _pricingServiceMock;
    private readonly Mock<IClock> _clockMock;
    private readonly CoversService _coversService;

    public CoversServiceTests()
    {
        _coversRepositoryMock = new Mock<ICoversRepository>();
        _pricingServiceMock = new Mock<IPricingService>();
        _clockMock = new Mock<IClock>();
        _coversService = new CoversService(_coversRepositoryMock.Object, _pricingServiceMock.Object, _clockMock.Object);
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
            var request = new CreateCoverRequest(startDate, endDate, AnyCoverType);
            StubUtcNow(utcNow);

            await AssertExtended.ThrowsArgumentExceptionAsync(
                () => _coversService.CreateCoverAsync(request),
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
            var request = new CreateCoverRequest(startDate, endDate, AnyCoverType);
            StubUtcNow(utcNow);

            await AssertExtended.ThrowsArgumentExceptionAsync(
                () => _coversService.CreateCoverAsync(request),
                "End date cannot be in the past."
            );
        }
    }

    [Fact]
    public async Task ThrowsExceptionWhenEndDateNotItThePastButGoesBeforeStartDate()
    {
        await Test(
            UtcDateTime(2000, 01, 10),
            Date(2010, 01, 10),
            Date(2010, 01, 08)
        );
        await Test(
            UtcDateTime(2000, 01, 10),
            Date(2010, 01, 10),
            Date(2010, 01, 09)
        );
        await Test(
            UtcDateTime(1981, 06, 07),
            Date(2010, 01, 10),
            Date(2010, 01, 09)
        );
        await Test(
            UtcDateTime(1981, 06, 07),
            Date(1981, 06, 10),
            Date(1981, 06, 09)
        );
        await Test(
            UtcDateTime(1981, 06, 07),
            Date(1981, 06, 11),
            Date(1981, 06, 10)
        );
    
        async Task Test(DateTime utcNow, DateOnly startDate, DateOnly endDate)
        {
            var request = new CreateCoverRequest(startDate, endDate, AnyCoverType);
            StubUtcNow(utcNow);
    
            await AssertExtended.ThrowsArgumentExceptionAsync(
                () => _coversService.CreateCoverAsync(request),
                "End date cannot be earlier than the start date."
            );
        }
    }

    [Fact]
    public async Task ThrowsExceptionWhenCoverPeriodExceedsASingleYear()
    {
        // NOTE: Making an assumption that a 1-year period for insurance takes
        // into consideration the fact that different years might have a different
        // number of days. In this situation, the insurance is considered valid
        // right until the same day happens the next year after the insurance start.
        await Test(
            UtcDateTime(2000, 01, 01),
            Date(2000, 01, 01),
            Date(2001, 01, 01)
        );
        await Test(
            UtcDateTime(1972, 01, 01),
            Date(1972, 01, 01),
            Date(1973, 01, 01)
        );
        await Test(
            UtcDateTime(1972, 01, 01),
            Date(1972, 01, 01),
            Date(1999, 01, 01)
        );
        await Test(
            UtcDateTime(1972, 01, 01),
            Date(1972, 01, 01),
            Date(1999, 02, 03)
        );
        await Test(
            UtcDateTime(1972, 01, 01),
            Date(1998, 02, 03),
            Date(1999, 02, 03)
        );

        async Task Test(DateTime utcNow, DateOnly startDate, DateOnly endDate)
        {
            var request = new CreateCoverRequest(startDate, endDate, AnyCoverType);
            StubUtcNow(utcNow);

            await AssertExtended.ThrowsArgumentExceptionAsync(
                () => _coversService.CreateCoverAsync(request),
                "Total insurance period cannot exceed 1 year."
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
            var request = new CreateCoverRequest(startDate, endDate, coverType);
            StubUtcNow(utcNow);
            StubPremium(startDate, endDate, coverType, premium);

            await _coversService.CreateCoverAsync(request);

            _coversRepositoryMock.Verify(x => x.AddAsync(new NewCoverInfo(startDate, endDate, coverType, premium)));
        }
    }

    [Fact]
    public async Task ReturnsCoverByIdFromRepository()
    {
        var id = Guid.NewGuid();
        var cover = RandomCover();
        StubFindCoverById(id, cover);

        var returnedCover = await _coversService.GetCoverAsync(id);

        Assert.Equal(cover, returnedCover);
    }

    [Fact]
    public async Task ReturnsAllCoversFromRepository()
    {
        var covers = new[] { RandomCover(), RandomCover() };
        StubGetAllCovers(covers);

        var returnedCovers = await _coversService.GetAllCoversAsync();

        Assert.Equal(covers, returnedCovers);
    }

    [Fact]
    public async Task DeletesCoverByIdInRepository()
    {
        var id = Guid.NewGuid();

        await _coversService.DeleteCoverAsync(id);

        _coversRepositoryMock.Verify(x => x.DeleteAsync(id));
    }

    [Fact]
    public async Task ReturnsCoverReturnedByRepositoryDelete()
    {
        var id = Guid.NewGuid();
        var cover = RandomCover();
        StubDeleteCover(id, cover);

        var returnedCover = await _coversService.DeleteCoverAsync(id);

        Assert.Equal(cover, returnedCover);
    }

    private Cover RandomCover()
    {
        return new Cover
        {
            Id = Guid.NewGuid(),
            StartDate = TestData.RandomDate(),
            EndDate = TestData.RandomDate(),
            Type = TestData.RandomEnum<CoverType>(),
            Premium = TestData.RandomInt(100)
        };
    }

    private void StubFindCoverById(Guid id, Cover? coverToReturn)
    {
        _coversRepositoryMock
            .Setup(x => x.FindByIdAsync(id))
            .ReturnsAsync(coverToReturn);
    }

    private void StubGetAllCovers(IEnumerable<Cover> coversToReturn)
    {
        _coversRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(coversToReturn);
    }

    private void StubDeleteCover(Guid id, Cover? deletedCoverToReturn)
    {
        _coversRepositoryMock
            .Setup(x => x.DeleteAsync(id))
            .ReturnsAsync(deletedCoverToReturn);
    }

    private void StubUtcNow(DateTime utcNow)
    {
        _clockMock
            .Setup(x => x.UtcNow())
            .Returns(utcNow);
    }

    private void StubPremium(DateOnly startDate, DateOnly endDate, CoverType coverType, decimal premium)
    {
        _pricingServiceMock
            .Setup(x => x.CalculatePremium(startDate, endDate, coverType))
            .Returns(premium);
    }
}