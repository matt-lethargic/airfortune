using System.Text.Json.Serialization;

namespace AirFortune.Airtable;

public class Thumbnails
{
    [JsonPropertyName("small")]
    [JsonInclude]
    public Thumbnail Small { get; internal set; }

    [JsonPropertyName("large")]
    [JsonInclude]
    public Thumbnail Large { get; internal set; }
}