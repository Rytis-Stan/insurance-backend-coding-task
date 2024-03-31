﻿using System.Net;
using Claims.Domain;
using Claims.Dto;
using Xunit;

namespace Claims.Tests;

public class ClaimsControllerTests : ControllerTests
{
    [Fact]
    public async Task CoversPostReturnsNewlyCreatedCover()
    {
        var utcNow = DateTime.UtcNow;
        // NOTE: Start and end date should start at least 1 day after UTC Now to avoid the
        // current date changing while the endpoint is being called (can happen if the
        // test starts running just before a day's end).
        var startDate = DateOnly.FromDateTime(utcNow).AddDays(TestData.RandomInt(1, 10));
        var endDate = startDate.AddDays(TestData.RandomInt(100));
        var coverType = TestData.RandomEnum<CoverType>();
        
        var response = await Client.PostAsync("/Covers", new CreateCoverRequestDto(startDate, endDate, coverType));

        response.EnsureSuccessStatusCode();
        var cover = await response.ReadContentAsync<CoverDto>();

        Assert.NotNull(cover);
        Assert.NotEqual(Guid.Empty, cover.Id);
        Assert.Equal(startDate, cover.StartDate);
        Assert.Equal(endDate, cover.EndDate);
        Assert.Equal(coverType, cover.Type);
        Assert.Equal(100.00m, cover.Premium);
    }

    [Fact]
    public async Task CoversGetReturnEmptyCoverCollectionWhenNoClaimsAdded()
    {
        var response = await Client.GetAsync("/Covers");
        
        response.EnsureSuccessStatusCode();
        var covers = await response.ReadContentAsync<CoverDto[]>();
        Assert.Empty(covers);
    }

    [Fact]
    public async Task CoversGetWithIdReturnsNotFoundWhenNoCoverExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await Client.GetAsync($"/Covers/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CoversDeleteWithIdReturnsNotFoundWhenNoCoverExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await Client.DeleteAsync($"/Covers/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("2001-01-01", "2001-01-01", CoverType.Yacht, 1375.00)]
    public async Task CoversPremiumGetReturnsCalculatedPremiumForGivenPeriodBasedOnCoverType(
        string startDate, string endDate, CoverType coverType, decimal expectedPremium)
    {
        var response = await Client.GetAsync($"/Covers/Premium/?startDate={startDate}&endDate={endDate}&coverType={coverType}");

        response.EnsureSuccessStatusCode();
        var premium = await response.ReadContentAsync<decimal>();
        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public async Task ClaimsPostReturnsNewlyCreatedClaim()
    {
        var coverId = Guid.NewGuid();
        var name = TestData.RandomString("name");
        var claimType = TestData.RandomEnum<ClaimType>();
        var damageCost = TestData.RandomInt(10_000);
        var dateTime = TestData.RandomUtcDateTime();

        var response = await Client.PostAsync("/Claims", new CreateClaimRequestDto(coverId, name, claimType, damageCost, dateTime));

        response.EnsureSuccessStatusCode();
        var claim = response.ReadContentAsync<ClaimDto>();
        Assert.NotNull(claim);
    }

    [Fact]
    public async Task ClaimsGetReturnEmptyClaimCollectionWhenNoClaimsAdded()
    {
        var response = await Client.GetAsync("/Claims");

        response.EnsureSuccessStatusCode();
        var claims = await response.ReadContentAsync<ClaimDto[]>();
        Assert.Empty(claims);
    }

    [Fact]
    public async Task ClaimsGetWithIdReturnsNotFoundWhenNoClaimExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await Client.GetAsync($"/Claims/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ClaimsDeleteWithIdReturnsNotFoundWhenNoClaimExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await Client.DeleteAsync($"/Claims/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
