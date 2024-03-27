using Claims.Domain;
using Newtonsoft.Json;

namespace Claims.DataAccess;

public class CoverJson
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "startDate")]
    public DateOnly StartDate { get; set; }

    [JsonProperty(PropertyName = "endDate")]
    public DateOnly EndDate { get; set; }

    [JsonProperty(PropertyName = "claimType")]
    public CoverType Type { get; set; }

    // TODO: Use decimal???
    [JsonProperty(PropertyName = "premium")]
    public decimal Premium { get; set; }
}
