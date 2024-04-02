using System.Net;
using Claims.Api.Dto;
using Claims.Domain;
using Claims.Testing;
using Xunit;

namespace Claims.Api.Tests;

public partial class ApiTests : IDisposable
{
    [Fact]
    public async Task ClaimsPostReturnsNewlyCreatedClaim()
    {
        var utcNow = DateTime.UtcNow;
        var coverStartDate = DateOnly.FromDateTime(utcNow).AddDays(TestData.RandomInt(1, 100));
        var coverPeriodDurationInDays = 200;
        var coverEndDate = coverStartDate.AddDays(TestData.RandomInt(coverPeriodDurationInDays - 1));
        var coverType = TestData.RandomEnum<CoverType>();
        var coversResponse = await _client.PostAsync("/Covers", new CreateCoverRequestDto(coverStartDate, coverEndDate, coverType));
        coversResponse.EnsureSuccessStatusCode();
        var cover = await coversResponse.ReadContentAsync<CoverDto>();

        var coverId = cover!.Id;
        var name = TestData.RandomString("name");
        var claimType = TestData.RandomEnum<ClaimType>();
        var damageCost = TestData.RandomInt(10_000);
        var created = TestValueBuilder.UtcDateTime(coverStartDate);

        var claimsResponse = await _client.PostAsync("/Claims", new CreateClaimRequestDto(coverId, name, claimType, damageCost, created));

        claimsResponse.EnsureSuccessStatusCode();
        var claim = await claimsResponse.ReadContentAsync<ClaimDto>();
        Assert.NotNull(claim);
        Assert.NotEqual(Guid.Empty, claim.Id);
        Assert.Equal(coverId, claim.CoverId);
        Assert.Equal(created, claim.Created);
        Assert.Equal(name, claim.Name);
        Assert.Equal(claimType, claim.Type);
        Assert.Equal(damageCost, claim.DamageCost);
    }

    [Fact]
    public async Task ClaimsGetReturnEmptyClaimCollectionWhenNoClaimsAdded()
    {
        var response = await _client.GetAsync("/Claims");

        response.EnsureSuccessStatusCode();
        var claims = await response.ReadContentAsync<ClaimDto[]>();
        Assert.NotNull(claims);
        Assert.Empty(claims);
    }

    [Fact]
    public async Task ClaimsGetWithIdReturnsNotFoundWhenNoClaimExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await _client.GetAsync($"/Claims/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ClaimsDeleteWithIdReturnsNoContentWhenNoClaimExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/Claims/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
