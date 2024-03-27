using Newtonsoft.Json;

namespace Claims.Domain;

public class Claim
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; init; }

    [JsonProperty(PropertyName = "coverId")]
    public string CoverId { get; init; }

    [JsonProperty(PropertyName = "created")]
    public DateTime Created { get; init; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; init; }

    [JsonProperty(PropertyName = "claimType")]
    public ClaimType Type { get; init; }

    [JsonProperty(PropertyName = "damageCost")]
    public decimal DamageCost { get; init; }
}

public enum ClaimType
{
    Collision = 0,
    Grounding = 1,
    BadWeather = 2,
    Fire = 3
}