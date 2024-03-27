using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Claims.Tests;

public class ClaimsControllerTests
{
    [Fact]
    public async Task Get_Claims()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/Claims");

        response.EnsureSuccessStatusCode();

        //TODO: Apart from ensuring 200 OK being returned, what else can be asserted?
    }

    private HttpClient CreateClient()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ => {});

        return application.CreateClient();
    }
}