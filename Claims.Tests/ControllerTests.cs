using System;
using System.Diagnostics.CodeAnalysis;
using Claims.Domain;
using Claims.Dto;
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
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/Claims", httpContent);
        return response;
    }
}