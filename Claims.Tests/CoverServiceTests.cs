using Claims.Domain;
using Claims.Dto;
using Claims.Infrastructure;
using Xunit;

namespace Claims.Tests;

public class CoverServiceTests
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
            UtcDateTime(2000, 01, 11),
            new DateOnly(2000, 01, 10)
        );

        async Task Test(DateTime utcNow, DateOnly startDate)
        {
            var endDate = DateOnly.FromDateTime(utcNow);
            var request = new CreateCoverRequestDto(startDate, endDate, AnyCoverType);
            var service = new CoversService(new CoversRepositoryStub(), new ClockStub(utcNow));

            await AssertThrowsArgumentExceptionAsync(
                () => service.CreateCoverAsync(request),
                "Start date cannot be in the past."
            );
        }
    }

    private async Task AssertThrowsArgumentExceptionAsync(Func<Task> action, string expectedMessage)
    {
        var exception = await Assert.ThrowsAsync<ArgumentException>(action);
        Assert.Equal(expectedMessage, exception.Message);
    }

    private DateTime UtcDateTime(int year, int month, int day)
    {
        return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
    }

    private class CoversRepositoryStub : ICoversRepository
    {
        public Task<Cover> AddAsync(INewCoverInfo item)
        {
            throw new NotImplementedException();
        }

        public Task<Cover?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Cover>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Cover> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
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
