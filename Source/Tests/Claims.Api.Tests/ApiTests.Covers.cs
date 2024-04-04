using Claims.Api.Dto;
using Claims.Domain;
using Claims.Testing;
using System.Net;
using Xunit;

namespace Claims.Api.Tests;

public partial class ApiTests
{
    [Theory]
    [InlineData(1, CoverType.BulkCarrier, 1625.00)]
    [InlineData(179, CoverType.Tanker, 330037.50)]
    [InlineData(182, CoverType.Yacht, 239717.50)]
    public async Task CoversPostReturnsNewlyCreatedCover(int periodDurationInDays, CoverType coverType, decimal expectedPremium)
    {
        var utcNow = DateTime.UtcNow;
        // NOTE: Start and end date should start at least 1 day after UTC Now to avoid the
        // current date changing while the endpoint is being called (can happen if the
        // test starts running just before a day's end).
        var startDate = DateOnly.FromDateTime(utcNow).AddDays(TestData.RandomInt(1, 100));
        var endDate = startDate.AddDays(periodDurationInDays - 1);

        var response = await CoversPostAsync(startDate, endDate, coverType);

        response.EnsureSuccessStatusCode();
        var cover = await response.ReadContentAsync<CoverDto>();

        Assert.NotNull(cover);
        Assert.NotEqual(Guid.Empty, cover.Id);
        Assert.Equal(startDate, cover.StartDate);
        Assert.Equal(endDate, cover.EndDate);
        Assert.Equal(coverType, cover.Type);
        Assert.Equal(expectedPremium, cover.Premium);
    }

    [Fact]
    public async Task CoversGetReturnEmptyCoverCollectionWhenNoClaimsAdded()
    {
        var response = await CoversGetAsync();

        response.EnsureSuccessStatusCode();
        var covers = await response.ReadContentAsync<CoverDto[]>();
        Assert.NotNull(covers);
        Assert.Empty(covers);
    }

    [Fact]
    public async Task CoversGetWithIdReturnsNotFoundWhenNoCoverExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await CoversGetAsync(id);

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
    [InlineData("2001-01-01", "2001-01-01", CoverType.Yacht, 1375.00)]
    [InlineData("2024-03-02", "2024-03-31", CoverType.ContainerShip, 48750)]
    [InlineData("1995-06-05", "1995-12-04", CoverType.Tanker, 337331.25)]
    public async Task CoversPremiumGetReturnsCalculatedPremiumForGivenPeriodBasedOnCoverType(
        string startDate, string endDate, CoverType coverType, decimal expectedPremium)
    {
        var response = await CoversPremiumGetAsync(startDate, endDate, coverType);

        response.EnsureSuccessStatusCode();
        var premium = await response.ReadContentAsync<decimal>();
        Assert.Equal(expectedPremium, premium);
    }
}
