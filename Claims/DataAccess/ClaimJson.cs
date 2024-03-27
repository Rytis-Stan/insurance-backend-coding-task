using Claims.Domain;
using Newtonsoft.Json;

namespace Claims.DataAccess;

public class ClaimJson
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "coverId")]
    public string CoverId { get; set; }

    [JsonProperty(PropertyName = "created")]
    public DateTime Created { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "claimType")]
    public ClaimType Type { get; set; }

    [JsonProperty(PropertyName = "damageCost")]
    public decimal DamageCost { get; set; }
}
