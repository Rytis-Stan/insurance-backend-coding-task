using Newtonsoft.Json;

namespace Claims.Persistence.Items;

public class ClaimItem
{
    [JsonProperty(PropertyName = "id")]
    public required string Id { get; init; }

    public required string CoverId { get; init; }
    public required string Name { get; init; }
    public required ClaimItemType Type { get; init; }
    public required decimal DamageCost { get; init; }
    public required DateTime Created { get; init; }
}
