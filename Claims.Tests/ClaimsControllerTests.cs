using System.Net;
using Claims.Domain;
using Claims.Dto;
using Xunit;

namespace Claims.Tests;

public class ClaimsControllerTests : ControllerTests
{
    [Fact]
    public async Task ClaimsPostReturnsNewlyCreatedClaim()
    {
        var coverId = Guid.NewGuid();
        var name = "someRandomName";
        var claimType = ClaimType.BadWeather;
        var damageCost = 123;
        var dateTime = new DateTime(2000, 01, 01);

        var response = await Client.PostAsync("/Claims", new CreateClaimRequestDto(coverId, name, claimType, damageCost, dateTime));

        response.EnsureSuccessStatusCode();

        var claim = response.ReadContentAsync<object>();

        Assert.NotNull(claim);
    }

    [Fact]
    public async Task ClaimsGetReturnEmptyClaimCollectionWhenNoClaimsAdded()
    {
        var response = await Client.GetAsync("/Claims");

        response.EnsureSuccessStatusCode();

        var claims = await response.ReadContentAsync<object[]>();

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
