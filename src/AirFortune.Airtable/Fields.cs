using System.Text.Json.Serialization;

namespace AirFortune.Airtable;

public class Fields
{
    [JsonPropertyName("fields")]
    public Dictionary<string, object> FieldsCollection { get; set; } = new();

    public void AddField(string fieldName, object fieldValue)
    {
        FieldsCollection.Add(fieldName, fieldValue);
    }
}