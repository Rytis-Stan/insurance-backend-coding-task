using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace Claims.Tests;

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

    public static async Task<T?> ReadContentAsync<T>(this HttpResponseMessage response)
        where T : class
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content);
    }
}
