using Claims.Domain;
using Claims.Dto;
using Claims.Infrastructure;
using Xunit;

namespace Claims.Tests;

public class CoversServiceTests
{
    private const CoverType AnyCoverType = CoverType.PassengerShip;

    [Fact]
    public async Task ThrowsExceptionWhenCreatingACoverWithStartDateThatIsInThePast()
    {
        await Test(
            UtcDateTime(2000, 01, 10),
            new DateOnly(2000, 01, 08)
        );
        await Test(
            UtcDateTime(2000, 01, 10),
            new DateOnly(2000, 01, 09)
        );
        await Test(
            UtcDateTime(1998, 07, 06),
            new DateOnly(1998, 07, 05)
        );
        await Test(
            UtcDateTime(1998, 07, 06),
            new DateOnly(1996, 05, 04)
        );

        async Task Test(DateTime utcNow, DateOnly startDate)
        {
            var endDate = DateOnly.FromDateTime(utcNow);
            var request = new CreateCoverRequestDto(startDate, endDate, AnyCoverType);
            var service = new CoversService(new CoversRepositoryStub(), new ClockStub(utcNow));

            await AssertExtended.ThrowsArgumentExceptionAsync(
                () => service.CreateCoverAsync(request),
                "Start date cannot be in the past."
            );
        }
    }

    [Fact]
    public async Task ThrowsExceptionWhenCreatingACoverWithEndDateThatIsInThePast()
    {
        await Test(
            UtcDateTime(2000, 01, 10),
            new DateOnly(2000, 01, 08)
        );
        await Test(
            UtcDateTime(2000, 01, 10),
            new DateOnly(2000, 01, 09)
        );
        await Test(
            UtcDateTime(1998, 07, 06),
            new DateOnly(1998, 07, 05)
        );
        await Test(
            UtcDateTime(1998, 07, 06),
            new DateOnly(1996, 05, 04)
        );

        async Task Test(DateTime utcNow, DateOnly endDate)
        {
            var startDate = DateOnly.FromDateTime(utcNow);
            var request = new CreateCoverRequestDto(startDate, endDate, AnyCoverType);
            var service = new CoversService(new CoversRepositoryStub(), new ClockStub(utcNow));

            await AssertExtended.ThrowsArgumentExceptionAsync(
                () => service.CreateCoverAsync(request),
                "End date cannot be in the past."
            );
        }
    }

    [Fact]
    public async Task ThrowsExceptionWhenEndDateNotItThePastButGoesBeforeStartDate()
    {
        await Test(
            UtcDateTime(2000, 01, 10),
            new DateOnly(2010, 01, 10),
            new DateOnly(2010, 01, 08)
        );
        await Test(
            UtcDateTime(2000, 01, 10),
            new DateOnly(2010, 01, 10),
            new DateOnly(2010, 01, 09)
        );
        await Test(
            UtcDateTime(1981, 06, 07),
            new DateOnly(2010, 01, 10),
            new DateOnly(2010, 01, 09)
        );
        await Test(
            UtcDateTime(1981, 06, 07),
            new DateOnly(1981, 06, 10),
            new DateOnly(1981, 06, 09)
        );
        await Test(
            UtcDateTime(1981, 06, 07),
            new DateOnly(1981, 06, 11),
            new DateOnly(1981, 06, 10)
        );

        async Task Test(DateTime utcNow, DateOnly startDate, DateOnly endDate)
        {
            var request = new CreateCoverRequestDto(startDate, endDate, AnyCoverType);
            var service = new CoversService(new CoversRepositoryStub(), new ClockStub(utcNow));

            await AssertExtended.ThrowsArgumentExceptionAsync(
                () => service.CreateCoverAsync(request),
                "End date cannot be earlier than the start date."
            );
        }
    }

    private DateTime UtcDateTime(int year, int month, int day)
    {
        return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
    }

    private class ClockStub : IClock
    {
        private readonly DateTime _utcNow;

        public ClockStub(DateTime utcNow)
        {
            _utcNow = utcNow;
        }

        public DateTime UtcNow()
        {
            return _utcNow;
        }
    }
}