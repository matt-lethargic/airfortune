using System.Text.Json.Serialization;

namespace AirFortune.Airtable.Responses;

public abstract class AirtableRecordListBase
{
    [JsonPropertyName("offset")]
    [JsonInclude]
    public string? Offset { get; internal set; }
}