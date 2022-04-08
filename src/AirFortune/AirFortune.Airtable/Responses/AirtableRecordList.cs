using System.Text.Json.Serialization;

namespace AirFortune.Airtable.Responses;

public class AirtableRecordList : AirtableRecordListBase
{
    [JsonPropertyName("records")]
    [JsonInclude]
    public AirtableRecord[]? Records { get; internal set; }
}

public class AirtableRecordList<T> : AirtableRecordListBase
{
    [JsonPropertyName("records")]
    [JsonInclude]
    public IEnumerable<AirtableRecord>? Records { get; internal set; }
}