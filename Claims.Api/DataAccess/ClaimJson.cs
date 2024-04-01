using Claims.Domain;
using Newtonsoft.Json;

namespace Claims.DataAccess;

public class ClaimJson
{
    [JsonProperty(PropertyName = "id")]
    public required string Id { get; init; }

    public required string CoverId { get; init; }
    public required string Name { get; init; }
    public required ClaimType Type { get; init; }
    public required decimal DamageCost { get; init; }
    public required DateTime Created { get; init; }
}
