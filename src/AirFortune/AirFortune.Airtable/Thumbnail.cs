using System.Text.Json.Serialization;

namespace AirFortune.Airtable;

public class Thumbnail
{
    [JsonPropertyName("url")]
    [JsonInclude]
    public string Url { get; internal set; }

    [JsonPropertyName("width")]
    [JsonInclude]
    public long Width { get; internal set; }

    [JsonPropertyName("height")]
    [JsonInclude]
    public long Height { get; internal set; }
}