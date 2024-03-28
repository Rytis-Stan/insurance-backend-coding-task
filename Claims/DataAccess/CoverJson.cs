using Claims.Domain;
using Newtonsoft.Json;

namespace Claims.DataAccess;

public class CoverJson
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; init; }

    [JsonProperty(PropertyName = "startDate")]
    public DateOnly StartDate { get; init; }

    [JsonProperty(PropertyName = "endDate")]
    public DateOnly EndDate { get; init; }

    [JsonProperty(PropertyName = "claimType")]
    public CoverType Type { get; init; }

    // TODO: Use decimal???
    [JsonProperty(PropertyName = "premium")]
    public decimal Premium { get; init; }
}
