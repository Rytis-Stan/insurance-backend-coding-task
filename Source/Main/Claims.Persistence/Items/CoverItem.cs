using Newtonsoft.Json;

namespace Claims.Persistence.Items;

public class CoverItem
{
    [JsonProperty(PropertyName = "id")]
    public required string Id { get; init; }

    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public required CoverItemType Type { get; init; }
    public required decimal Premium { get; init; }
}
