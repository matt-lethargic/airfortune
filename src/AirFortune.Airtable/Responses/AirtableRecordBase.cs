using System.Text.Json.Serialization;

namespace AirFortune.Airtable.Responses;

public abstract class AirtableRecordBase
{
    [JsonPropertyName("id")]
    [JsonInclude]
    public string? Id { get; internal set; }

    [JsonPropertyName("createdTime")]
    [JsonInclude]
    public DateTime CreatedTime { get; internal set; }
}