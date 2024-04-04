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
        var cover = await CreateRandomCover();

        var name = TestData.RandomString("name");
        var claimType = TestData.RandomEnum<ClaimType>();
        var damageCost = TestData.RandomInt(10_000);
        var created = TestValueBuilder.UtcDateTime(cover.StartDate);

        var claimsResponse = await ClaimsPostAsync(cover.Id, name, claimType, damageCost, created);

        claimsResponse.EnsureSuccessStatusCode();
        var claim = await claimsResponse.ReadContentAsync<ClaimDto>();
        Assert.NotNull(claim);
        Assert.NotEqual(Guid.Empty, claim.Id);
        Assert.Equal(cover.Id, claim.CoverId);
        Assert.Equal(created, claim.Created);
        Assert.Equal(name, claim.Name);
        Assert.Equal(claimType, claim.Type);
        Assert.Equal(damageCost, claim.DamageCost);
    }

    [Fact]
    public async Task ClaimsGetReturnEmptyClaimCollectionWhenNoClaimsAdded()
    {
        var response = await ClaimsGetAsync();

        response.EnsureSuccessStatusCode();
        var claims = await response.ReadContentAsync<ClaimDto[]>();
        Assert.NotNull(claims);
        Assert.Empty(claims);
    }

    [Fact]
    public async Task ClaimsGetWithIdReturnsNotFoundWhenNoClaimExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await ClaimsGetAsync(id);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ClaimsDeleteWithIdReturnsNoContentWhenNoClaimExistsWithGivenId()
    {
        var id = Guid.NewGuid();

        var response = await ClaimsDeleteAsync(id);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // TODO: How should this method be named to make it clear that it does not just construct a DTO but actually calls an endpoint for the Cover creation?
    private async Task<CoverDto?> CreateRandomCover()
    {
        var utcNow = DateTime.UtcNow;
        var startDate = DateOnly.FromDateTime(utcNow).AddDays(TestData.RandomInt(1, 100));
        var periodDurationInDays = 200;
        var endDate = startDate.AddDays(TestData.RandomInt(periodDurationInDays - 1));
        var coverType = TestData.RandomEnum<CoverType>();
        var response = await CoversPostAsync(startDate, endDate, coverType);
        response.EnsureSuccessStatusCode();
        return await response.ReadContentAsync<CoverDto>();
    }
}
