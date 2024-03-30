using System.Net;
using Newtonsoft.Json;
using Xunit;

namespace Claims.Tests;

public class ClaimsControllerTests : ControllerTests
{
    [Fact]
    public async Task ClaimsGetReturnEmptyClaimCollectionWhenNoClaimsAdded()
    {
        var response = await Client.GetAsync("/Claims");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var claims = JsonConvert.DeserializeObject<object[]>(content);

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
