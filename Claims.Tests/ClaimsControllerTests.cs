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
        var startDate = TestData.RandomDate();
        var endDate = TestData.RandomDate();
        var coverType = TestData.RandomEnum<CoverType>();
        
        var response = await Client.PostAsync("/Covers", new CreateCoverRequestDto(startDate, endDate, coverType));

        response.EnsureSuccessStatusCode();
        var cover = await response.ReadContentAsync<CoverDto>();

        Assert.Equal(
            new CoverDto(Guid.NewGuid(), startDate, endDate, coverType, 100.00m),
            cover
        );
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
}
