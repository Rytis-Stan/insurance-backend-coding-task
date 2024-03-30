using Claims.Domain;
using Claims.Dto;
using Claims.Infrastructure;
using Moq;
using Xunit;
using static Claims.Tests.TestValueBuilder;

namespace Claims.Tests;

public class CoversServiceTests
{
    private const CoverType AnyCoverType = CoverType.PassengerShip;

    private readonly Mock<ICoversRepository> _coversRepositoryMock;
    private readonly Mock<IClock> _clockMock;
    private readonly CoversService _coversService;

    public CoversServiceTests()
    {
        _coversRepositoryMock = new Mock<ICoversRepository>();
        _clockMock = new Mock<IClock>();
        _coversService = new CoversService(_coversRepositoryMock.Object, _clockMock.Object);
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
            var request = new CreateCoverRequestDto(startDate, endDate, AnyCoverType);
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
            var request = new CreateCoverRequestDto(startDate, endDate, AnyCoverType);
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
            var request = new CreateCoverRequestDto(startDate, endDate, AnyCoverType);
            StubUtcNow(utcNow);

            await AssertExtended.ThrowsArgumentExceptionAsync(
                () => _coversService.CreateCoverAsync(request),
                "End date cannot be earlier than the start date."
            );
        }
    }

    [Fact]
    public async Task AddsCoverToRepositoryWhenCreatingAValidCover()
    {
        await Test(UtcDateTime(2000, 10, 10), Date(2000, 10, 20), Date(2000, 10, 20), CoverType.BulkCarrier, 123.45m);

        async Task Test(DateTime utcNow, DateOnly startDate, DateOnly endDate, CoverType coverType, decimal premium)
        {
            var request = new CreateCoverRequestDto(startDate, endDate, AnyCoverType);
            StubUtcNow(utcNow);

            await _coversService.CreateCoverAsync(request);

            _coversRepositoryMock.Verify(x => x.AddAsync(new NewCoverInfo(startDate, endDate, coverType, premium);
        }
    }

    private void StubUtcNow(DateTime utcNow)
    {
        _clockMock
            .Setup(x => x.UtcNow())
            .Returns(utcNow);
    }
}