using System.Text.Json.Serialization;

namespace AirFortune.Airtable.Responses;

public class AirtableAttachment
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    [JsonPropertyName("size")]
    public long? Size { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("thumbnails")]
    public Thumbnails? Thumbnails { get; set; }
}