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

    private static async Task<T> ReadRawContentAsync<T>(this HttpResponseMessage response)
    {
        var contentJson = await response.Content.ReadAsStringAsync();
        var content = DeserializeJson<T>(contentJson);
        Assert.NotNull(content); // TODO: Replace with some custom exception type?
        return content;
    }

    private static T? DeserializeJson<T>(string contentJson)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new JsonStringEnumConverter());
        return JsonSerializer.Deserialize<T?>(contentJson, options);
    }
}
