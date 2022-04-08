using System.Text.Json.Serialization;

namespace AirFortune.Airtable;

public class Sort
{
    [JsonPropertyName("fields")]
    public string Field { get; set; }

    [JsonPropertyName("direction")]
    public SortDirection Direction { get; set; }
}