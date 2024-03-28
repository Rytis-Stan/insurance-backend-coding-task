using Claims.Domain;
using Newtonsoft.Json;

namespace Claims.DataAccess;

public class ClaimJson
{
    [JsonProperty(PropertyName = "id")]
    public required string Id { get; init; }

    [JsonProperty(PropertyName = "coverId")]
    public required string CoverId { get; init; }

    [JsonProperty(PropertyName = "name")]
    public required string Name { get; init; }

    [JsonProperty(PropertyName = "claimType")]
    public required ClaimType Type { get; init; }

    [JsonProperty(PropertyName = "damageCost")]
    public required decimal DamageCost { get; init; }

    [JsonProperty(PropertyName = "created")]
    public required DateTime Created { get; init; }
}
