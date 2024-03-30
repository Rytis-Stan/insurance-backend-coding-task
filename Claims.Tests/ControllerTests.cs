using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

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

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> PostAsync<T>(this HttpClient client, [StringSyntax(StringSyntaxAttribute.Uri)] string requestUri, T content)
    {
        string json = JsonConvert.SerializeObject(content);
        var response = await client.PostAsync(requestUri, JsonContent(json));
        return response;
    }

    private static StringContent JsonContent(string json)
    {
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    public static async Task<T?> ReadContentAsync<T>(this HttpResponseMessage response)
        where T : class
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(content);
    }
}