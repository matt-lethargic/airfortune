using System.Text.Json.Serialization;

namespace AirFortune.Airtable.Responses;

internal class AirtableDeletedRecord
{
    [JsonPropertyName("deleted")]
    [JsonInclude]
    public bool Deleted { get; set; }

    [JsonPropertyName("id")]
    [JsonInclude]
    public string Id { get; set; }
}