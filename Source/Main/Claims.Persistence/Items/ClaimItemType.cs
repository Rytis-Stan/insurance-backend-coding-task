using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Claims.Persistence.Items;

[JsonConverter(typeof(StringEnumConverter))]
public enum ClaimItemType
{
    Collision,
    Grounding,
    BadWeather,
    Fire
}
