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
}
