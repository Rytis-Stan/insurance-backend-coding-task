using Claims.Api.Contracts.Messages;
using Claims.Testing;
using System.Net;
using Claims.Api.Contracts.Dto;
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
        var coverType = TestData.RandomEnum<CoverDtoType>();
        var request = new CreateCoverRequest(startDate, endDate, coverType);

        var httpResponse = await CoversPostAsync(request);
    
        await AssertBadRequestAsync(httpResponse, "Start date cannot be in the past.");
    }

    [Fact]
    public async Task CoversPostReturnsBadRequestWhenEndDateIsInThePast()
    {
        var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = utcNow;
        var endDate = utcNow.AddDays(-TestData.RandomInt(1, 100));
        var coverType = TestData.RandomEnum<CoverDtoType>();
        var request = new CreateCoverRequest(startDate, endDate, coverType);

        var httpResponse = await CoversPostAsync(request);

        await AssertBadRequestAsync(httpResponse, "End date cannot be in the past.");
    }

    [Fact]
    public async Task CoversPostReturnsBadRequestWhenEndDateNotItThePastButGoesBeforeStartDate()
    {
        var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = utcNow.AddDays(TestData.RandomInt(101, 200));
        var endDate = utcNow.AddDays(TestData.RandomInt(1, 100));
        var coverType = TestData.RandomEnum<CoverDtoType>();
        var request = new CreateCoverRequest(startDate, endDate, coverType);

        var httpResponse = await CoversPostAsync(request);

        await AssertBadRequestAsync(httpResponse, "End date cannot be earlier than the start date.");
    }

    [Fact]
    public async Task CoversPostReturnsBadRequestWhenCoverPeriodExceedsASingleYearByExactlyOneDay()
    {
        var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = utcNow.AddDays(TestData.RandomInt(1, 100));
        var endDate = OneYearPlus1DayCoverPeriodEnd(startDate);
        var coverType = TestData.RandomEnum<CoverDtoType>();
        var request = new CreateCoverRequest(startDate, endDate, coverType);

        var httpResponse = await CoversPostAsync(request);

        await AssertBadRequestAsync(httpResponse, "Total insurance period cannot exceed 1 year.");
    }

    [Fact]
    public async Task CoversPostReturnsBadRequestWhenCoverPeriodExceedsASingleYearByMoreThanOneDay()
    {
        var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = utcNow.AddDays(TestData.RandomInt(1, 100));
        var endDate = OneYearPlus1DayCoverPeriodEnd(startDate).AddDays(TestData.RandomInt(1, 90));
        var coverType = TestData.RandomEnum<CoverDtoType>();
        var request = new CreateCoverRequest(startDate, endDate, coverType);

        var httpResponse = await CoversPostAsync(request);

        await AssertBadRequestAsync(httpResponse, "Total insurance period cannot exceed 1 year.");
    }

    [Theory]
    [InlineData(1, CoverDtoType.BulkCarrier, 1625.00)]
    [InlineData(179, CoverDtoType.Tanker, 330037.50)]
    [InlineData(182, CoverDtoType.Yacht, 239717.50)]
    public async Task CoversPostReturnsNewlyCreatedCoverWhenRequestValid(int periodDurationInDays, CoverDtoType coverType, decimal expectedPremium)
    {
        var request = RandomCreateCoverRequest(DateTime.UtcNow, periodDurationInDays, coverType);

        var httpResponse = await CoversPostAsync(request);

        var response = await httpResponse.ReadContentAsync<CreateCoverResponse>(HttpStatusCode.Created);
        var cover = response.Cover;
        Assert.NotEqual(Guid.Empty, cover.Id);
        Assert.Equal(request.StartDate, cover.StartDate);
        Assert.Equal(request.EndDate, cover.EndDate);
        Assert.Equal(coverType, cover.Type);
        Assert.Equal(expectedPremium, cover.Premium);
    }

    [Fact]
    public async Task CoversPostReturnsLocationOfNewCoverThatCanBeUsedToRetrieveItLaterWhenRequestIsValid()
    {
        var request = RandomCreateCoverRequest(DateTime.UtcNow);

        var httpCreateResponse = await CoversPostAsync(request);

        var httpGetResponse = await _client.GetAsync(httpCreateResponse.Headers.Location);
        var createResponse = await httpCreateResponse.ReadContentAsync<CreateCoverResponse>(HttpStatusCode.Created);
        var getResponse = await httpGetResponse.OkReadContentAsync<GetCoverResponse>();
        Assert.Equal(createResponse.Cover, getResponse.Cover);
    }

    [Fact]
    public async Task CoversGetReturnEmptyCoverCollectionWhenNoClaimsAdded()
    {
        var httpResponse = await CoversGetAsync();

        var response = await httpResponse.OkReadContentAsync<GetCoversResponse>();
        Assert.Empty(response.Covers);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public async Task CoversGetReturnsAllCoversWhenCoversAdded(int coverCount)
    {
        var createdCovers = await CreateRandomCoversAsync(coverCount);

        var httpResponse = await CoversGetAsync();

        var response = await httpResponse.OkReadContentAsync<GetCoversResponse>();
        AssertExtended.EqualIgnoreOrder(createdCovers, response.Covers);
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

        var httpResponse = await CoversGetAsync();

        var response = await httpResponse.OkReadContentAsync<GetCoversResponse>();
        AssertExtended.EqualIgnoreOrder(createdCoversToKeep, response.Covers);
    }

    [Fact]
    public async Task CoversGetWithIdReturnsNotFoundWhenNoCoverCreatedWithGivenId()
    {
        var id = Guid.NewGuid();

        var httpResponse = await CoversGetAsync(id);

        Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
    }

    [Fact]
    public async Task CoversGetWithIdReturnsCoverWhenItWasAlreadyCreatedPreviously()
    {
        var createdCover = await CreateRandomCoverAsync();

        var httpResponse = await CoversGetAsync(createdCover.Id);

        var response = await httpResponse.OkReadContentAsync<GetCoverResponse>();
        Assert.Equal(createdCover, response.Cover);
    }

    [Fact]
    public async Task CoversGetWithIdReturnsNotFoundWhenCoverCreatedButLaterDeleted()
    {
        var createdCover = await CreateRandomCoverAsync();
        await CoversDeleteAsync(createdCover.Id);

        var httpResponse = await CoversGetAsync(createdCover.Id);

        Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
    }

    [Fact]
    public async Task CoversDeleteWithIdReturnsNoContentWhenNoCoverExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var httpResponse = await CoversDeleteAsync(id);

        Assert.Equal(HttpStatusCode.NoContent, httpResponse.StatusCode);
    }

    [Fact]
    public async Task CoversPremiumGetReturnsBadRequestWhenEndDateGoesBeforeStartDate()
    {
        var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = utcNow.AddDays(TestData.RandomInt(101, 200));
        var endDate = utcNow.AddDays(TestData.RandomInt(1, 100));
        var coverType = TestData.RandomEnum<CoverDtoType>();

        var httpResponse = await CoversPremiumGetAsync(startDate, endDate, coverType);

        await AssertBadRequestAsync(httpResponse, "End date cannot be earlier than the start date.");
    }

    [Fact]
    public async Task CoversPremiumGetReturnsBadRequestWhenCoverPeriodExceedsASingleYearByExactlyOneDay()
    {
        var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = utcNow.AddDays(TestData.RandomInt(1, 100));
        var endDate = OneYearPlus1DayCoverPeriodEnd(startDate);
        var coverType = TestData.RandomEnum<CoverDtoType>();

        var httpResponse = await CoversPremiumGetAsync(startDate, endDate, coverType);

        await AssertBadRequestAsync(httpResponse, "Total insurance period cannot exceed 1 year.");
    }

    [Fact]
    public async Task CoversPremiumGetReturnsBadRequestWhenCoverPeriodExceedsASingleYearByMoreThanOneDay()
    {
        var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = utcNow.AddDays(TestData.RandomInt(1, 100));
        var endDate = OneYearPlus1DayCoverPeriodEnd(startDate).AddDays(TestData.RandomInt(1, 90));
        var coverType = TestData.RandomEnum<CoverDtoType>();

        var httpResponse = await CoversPremiumGetAsync(startDate, endDate, coverType);

        await AssertBadRequestAsync(httpResponse, "Total insurance period cannot exceed 1 year.");
    }

    [Theory]
    [InlineData("2001-01-01", "2001-01-01", CoverDtoType.Yacht, 1375.00)]
    [InlineData("2024-03-02", "2024-03-31", CoverDtoType.ContainerShip, 48750)]
    [InlineData("1995-06-05", "1995-12-04", CoverDtoType.Tanker, 337331.25)]
    public async Task CoversPremiumGetReturnsPremiumForGivenPeriodBasedOnCoverTypeWhenRequestValid(
        string startDate, string endDate, CoverDtoType coverType, decimal expectedPremium)
    {
        var httpResponse = await CoversPremiumGetAsync(startDate, endDate, coverType);

        var response = await httpResponse.OkReadContentAsync<GetCoverPremiumResponse>();
        Assert.Equal(expectedPremium, response.Premium);
    }

    [Fact]
    public async Task CoversPremiumGetReturnsSamePremiumAsForNewlyCreatedCoverWhenRequestValidAndParametersAreSame()
    {
        var cover = await CreateRandomCoverAsync();

        var httpResponse = await CoversPremiumGetAsync(cover.StartDate, cover.EndDate, cover.Type);

        var response = await httpResponse.OkReadContentAsync<GetCoverPremiumResponse>();
        Assert.Equal(cover.Premium, response.Premium);
    }

    private static DateOnly OneYearPlus1DayCoverPeriodEnd(DateOnly startDate)
    {
        // NOTE: Please see the "CoverPricing" class for an explanation about the 1-year insurance duration.
        return startDate.AddYears(1);
    }

    // TODO: Move this method out of this class.
    private static CreateCoverRequest RandomCreateCoverRequest(DateTime utcNow)
    {
        return RandomCreateCoverRequest(
            utcNow,
            TestData.RandomInt(1, 90),
            TestData.RandomEnum<CoverDtoType>()
        );
    }

    private static CreateCoverRequest RandomCreateCoverRequest(DateTime utcNow, int periodDurationInDays, CoverDtoType coverType)
    {
        // NOTE: Start and end date should start at least 1 day after UTC Now to avoid the
        // current date changing while the endpoint is being called (can happen if the test
        // starts running just before a day's end).
        var startDate = DateOnly.FromDateTime(utcNow).AddDays(TestData.RandomInt(1, 100));
        var endDate = startDate.AddDays(periodDurationInDays - 1);
        return new CreateCoverRequest(startDate, endDate, coverType);
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
