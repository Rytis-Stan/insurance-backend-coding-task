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

    /// <summary>
    /// The method works as if calling "ReadContentAsync" with the status code set to "HttpStatusCode.OK".
    /// </summary>
    public static async Task<TContent> OkReadContentAsync<TContent>(this HttpResponseMessage response)
    {
        return await response.ReadContentAsync<TContent>(HttpStatusCode.OK);
    }

    /// <summary>
    /// Ensures that a given response contains the expected status code and if so, tries to deserialize
    /// its contents as if they were a serialized JSON version of the given type of "TContent".
    /// If the status code does not even match, no attempt is made to deserialize the contents and an
    /// exception is thrown.
    /// </summary>
    public static async Task<TContent> ReadContentAsync<TContent>(this HttpResponseMessage response, HttpStatusCode expectedStatusCode)
    {
        Assert.Equal(expectedStatusCode, response.StatusCode);
        var contentJson = await response.Content.ReadAsStringAsync();
        var content = DeserializeJson<TContent>(contentJson);
        Assert.NotNull(content);
        return content;
    }

    private static T? DeserializeJson<T>(string contentJson)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new JsonStringEnumConverter());
        return JsonSerializer.Deserialize<T?>(contentJson, options);
    }
}
