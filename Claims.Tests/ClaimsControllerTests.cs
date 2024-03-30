using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Claims.Tests;

public class ClaimsControllerTests : ControllerTests
{
    [Fact]
    public async Task Get_Claims()
    {
        var response = await Client.GetAsync("/Claims");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var claims = JsonConvert.DeserializeObject<object[]>(content);

        Assert.Empty(claims);
    }
}
