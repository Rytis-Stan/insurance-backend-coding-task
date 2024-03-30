using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Claims.Tests;

public abstract class ControllerTests : IDisposable
{
    protected readonly HttpClient Client;

    protected ControllerTests()
    {
        Client = CreateClient();
    }

    private HttpClient CreateClient()
    {
        var factory = new CustomWebApplicationFactory<Program>();
        return factory.CreateClient();
    }

    public void Dispose()
    {
        Client.Dispose();
    }

    private class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureHostConfiguration(config =>
            {
                var testConfig = new Dictionary<string, string?>
                {
                    {"CosmosDb:DatabaseName", "ClaimTestDb"}
                };
                config.AddInMemoryCollection(testConfig);
            });

            return base.CreateHost(builder);
        }
    }
}
