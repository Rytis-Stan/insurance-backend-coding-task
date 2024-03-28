using Claims.Domain;
using Newtonsoft.Json;

namespace Claims.DataAccess;

public class CoverJson
{
    [JsonProperty(PropertyName = "id")]
    public required string Id { get; init; }

    [JsonProperty(PropertyName = "startDate")]
    public required DateOnly StartDate { get; init; }

    [JsonProperty(PropertyName = "endDate")]
    public required DateOnly EndDate { get; init; }

    [JsonProperty(PropertyName = "claimType")]
    public required CoverType Type { get; init; }

    // TODO: Use decimal???
    [JsonProperty(PropertyName = "premium")]
    public required decimal Premium { get; init; }

    [JsonProperty(PropertyName = "created")]
    public required DateTime Created { get; init; }
}
