using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Claims.Api.Tests;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> PostAsync<T>(this HttpClient client, [StringSyntax(StringSyntaxAttribute.Uri)] string requestUri, T content)
    {
        string json = JsonSerializer.Serialize(content);
        var response = await client.PostAsync(requestUri, JsonContent(json));
        return response;
    }

    private static StringContent JsonContent(string json)
    {
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    public static async Task<T> ReadRawContentAsync<T>(this HttpResponseMessage response)
    {
        var contentJson = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new JsonStringEnumConverter());
        var content = JsonSerializer.Deserialize<T?>(contentJson, options);
        Assert.NotNull(content); // TODO: Replace with some custom exception type?
        return content;
    }

    // TODO: Fix method names.
    public static async Task<TContent> OkReadContentAsync<TContent>(this HttpResponseMessage response)
    {
        return await response.ReadContentAsync<TContent>(HttpStatusCode.OK);
    }

    public static async Task<TContent> ReadContentAsync<TContent>(this HttpResponseMessage response, HttpStatusCode expectedStatusCode)
    {
        Assert.Equal(expectedStatusCode, response.StatusCode);
        return await response.ReadRawContentAsync<TContent>();
    }
}
