using Claims.Api.Dto;
using Claims.Testing;
using System.Net;
using Xunit;

namespace Claims.Api.Tests;

public partial class ApiTests
{
    [Fact]
    public async Task CoversPostReturnsBadRequestWhenStartDateIsInThePast()
    {
        var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = utcNow.AddDays(-TestData.RandomInt(1, 100));
        var endDate = utcNow;
        var coverType = TestData.RandomEnum<CoverTypeDto>();
        var request = new CreateCoverRequestDto(startDate, endDate, coverType);

        var response = await CoversPostAsync(request);
    
        await AssertReturnedBadRequestAsync(response, "Start date cannot be in the past.");
    }

    [Fact]
    public async Task CoversPostReturnsBadRequestWhenEndDateIsInThePast()
    {
        var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = utcNow;
        var endDate = utcNow.AddDays(-TestData.RandomInt(1, 100));
        var coverType = TestData.RandomEnum<CoverTypeDto>();
        var request = new CreateCoverRequestDto(startDate, endDate, coverType);

        var response = await CoversPostAsync(request);

        await AssertReturnedBadRequestAsync(response, "End date cannot be in the past.");
    }

    [Fact]
    public async Task CoversPostReturnsBadRequestWhenEndDateNotItThePastButGoesBeforeStartDate()
    {
        var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = utcNow.AddDays(TestData.RandomInt(101, 200));
        var endDate = utcNow.AddDays(TestData.RandomInt(1, 100));
        var coverType = TestData.RandomEnum<CoverTypeDto>();
        var request = new CreateCoverRequestDto(startDate, endDate, coverType);

        var response = await CoversPostAsync(request);

        await AssertReturnedBadRequestAsync(response, "End date cannot be earlier than the start date.");
    }

    [Fact]
    public async Task CoversPostReturnsBadRequestWhenCoverPeriodExceedsASingleYearByExactlyOneDay()
    {
        var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = utcNow.AddDays(TestData.RandomInt(1, 100));
        // NOTE: Even though "AddYears(1)" adds exactly a single year "calendar-wise", in practice
        // the insurance period duration becomes 1 year + 1 extra day. This is due to the fact that
        // insurance works from the start of the day on "startDate" to the end of the day of "endDate"
        // (so if "startDate" and "endDate" match, it is still technically a single day of insurance).
        var endDate = startDate.AddYears(1);
        
        var coverType = TestData.RandomEnum<CoverTypeDto>();
        var request = new CreateCoverRequestDto(startDate, endDate, coverType);

        var response = await CoversPostAsync(request);

        await AssertReturnedBadRequestAsync(response, "Total insurance period cannot exceed 1 year.");
    }

    [Fact]
    public async Task CoversPostReturnsBadRequestWhenCoverPeriodExceedsASingleYearByMoreThanOneDay()
    {
        var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = utcNow.AddDays(TestData.RandomInt(1, 100));
        var endDate = startDate.AddYears(1).AddDays(TestData.RandomInt(1, 90));
        var coverType = TestData.RandomEnum<CoverTypeDto>();
        var request = new CreateCoverRequestDto(startDate, endDate, coverType);

        var response = await CoversPostAsync(request);

        await AssertReturnedBadRequestAsync(response, "Total insurance period cannot exceed 1 year.");
    }

    [Theory]
    [InlineData(1, CoverTypeDto.BulkCarrier, 1625.00)]
    [InlineData(179, CoverTypeDto.Tanker, 330037.50)]
    [InlineData(182, CoverTypeDto.Yacht, 239717.50)]
    public async Task CoversPostReturnsNewlyCreatedCoverWhenRequestValid(int periodDurationInDays, CoverTypeDto coverType, decimal expectedPremium)
    {
        var request = RandomCreateCoverRequestDto(DateTime.UtcNow, periodDurationInDays, coverType);

        var response = await CoversPostAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cover = await response.ReadContentAsync<CoverDto>();
        Assert.NotNull(cover);
        Assert.NotEqual(Guid.Empty, cover.Id);
        Assert.Equal(request.StartDate, cover.StartDate);
        Assert.Equal(request.EndDate, cover.EndDate);
        Assert.Equal(coverType, cover.Type);
        Assert.Equal(expectedPremium, cover.Premium);
    }

    [Fact]
    public async Task CoversGetReturnEmptyCoverCollectionWhenNoClaimsAdded()
    {
        var response = await CoversGetAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var covers = await response.ReadContentAsync<CoverDto[]>();
        Assert.NotNull(covers);
        Assert.Empty(covers);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public async Task CoversGetReturnsAllCoversWhenCoversAdded(int coverCount)
    {
        var createdCovers = await CreateRandomCoversAsync(coverCount);

        var response = await CoversGetAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var covers = await response.ReadContentAsync<CoverDto[]>();
        AssertExtended.EqualIgnoreOrder(createdCovers, covers);
    }

    [Theory]
    [InlineData(2, 1)]
    [InlineData(4, 1)]
    [InlineData(4, 2)]
    public async Task CoversGetReturnsNonDeletedCoversWhenCoversAddedAndSomeDeleted(int coverAddCount, int coverDeleteCount)
    {
        var createdCovers = (await CreateRandomCoversAsync(coverAddCount)).ToList();
        var createdCoversToKeep = createdCovers.Skip(coverDeleteCount);
        var createdCoversToDelete = createdCovers.Take(coverDeleteCount);
        await CoversDeleteMultipleAsync(createdCoversToDelete.Select(x => x.Id));

        var response = await CoversGetAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var covers = await response.ReadContentAsync<CoverDto[]>();
        AssertExtended.EqualIgnoreOrder(createdCoversToKeep, covers);
    }

    [Fact]
    public async Task CoversGetWithIdReturnsNotFoundWhenNoCoverCreatedWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await CoversGetAsync(id);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CoversGetWithIdReturnsCoverWhenItWasAlreadyCreatedPreviously()
    {
        var createdCover = await CreateRandomCoverAsync();

        var response = await CoversGetAsync(createdCover.Id);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cover = await response.ReadContentAsync<CoverDto>();
        Assert.Equal(createdCover, cover);
    }

    [Fact]
    public async Task CoversGetWithIdReturnsNotFoundWhenCoverCreatedButLaterDeleted()
    {
        var createdCover = await CreateRandomCoverAsync();
        await CoversDeleteAsync(createdCover.Id);

        var response = await CoversGetAsync(createdCover.Id);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CoversDeleteWithIdReturnsNoContentWhenNoCoverExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await CoversDeleteAsync(id);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Theory]
    [InlineData("2001-01-01", "2001-01-01", CoverTypeDto.Yacht, 1375.00)]
    [InlineData("2024-03-02", "2024-03-31", CoverTypeDto.ContainerShip, 48750)]
    [InlineData("1995-06-05", "1995-12-04", CoverTypeDto.Tanker, 337331.25)]
    public async Task CoversPremiumGetReturnsCalculatedPremiumForGivenPeriodBasedOnCoverType(
        string startDate, string endDate, CoverTypeDto coverType, decimal expectedPremium)
    {
        var response = await CoversPremiumGetAsync(startDate, endDate, coverType);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var premium = await response.ReadContentAsync<decimal>();
        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public async Task CoversPremiumGetReturnsSamePremiumAsForNewlyCreatedCoverWhenParametersAreTheSame()
    {
        var cover = await CreateRandomCoverAsync();

        var response = await CoversPremiumGetAsync(cover.StartDate, cover.EndDate, cover.Type);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var premium = await response.ReadContentAsync<decimal>();
        Assert.Equal(premium, cover.Premium);
    }

    // TODO: Move this method out of this class.
    private static CreateCoverRequestDto RandomCreateCoverRequestDto(DateTime utcNow)
    {
        return RandomCreateCoverRequestDto(
            utcNow,
            TestData.RandomInt(1, 90),
            TestData.RandomEnum<CoverTypeDto>()
        );
    }

    private static CreateCoverRequestDto RandomCreateCoverRequestDto(DateTime utcNow, int periodDurationInDays, CoverTypeDto coverType)
    {
        // NOTE: Start and end date should start at least 1 day after UTC Now to avoid the
        // current date changing while the endpoint is being called (can happen if the test
        // starts running just before a day's end).
        var startDate = DateOnly.FromDateTime(utcNow).AddDays(TestData.RandomInt(1, 100));
        var endDate = startDate.AddDays(periodDurationInDays - 1);
        return new CreateCoverRequestDto(startDate, endDate, coverType);
    }

    private async Task CoversDeleteMultipleAsync(IEnumerable<Guid> coverIds)
    {
        var tasks = coverIds.Select(CoversDeleteAsync);
        await Task.WhenAll(tasks);
    }

    private async Task<IEnumerable<CoverDto>> CreateRandomCoversAsync(int coverCount)
    {
        var tasks = Enumerable
            .Range(0, coverCount)
            .Select(_ => CreateRandomCoverAsync());
        return await Task.WhenAll(tasks);
    }
}
