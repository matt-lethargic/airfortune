using System.Text.Json.Serialization;

namespace AirFortune.Airtable;

public class IdFields : Fields
{
    public IdFields(string id)
    {
        Id = id;
    }

    // Note: System.Text.Json's Serialization includes Properties by default
    // So it good practice to use Property instead of Fields.
    // Change 'field' to 'property' but not changing the case of 'i' in 'Id'
    // to keep backward compatiblity.
    [JsonPropertyName("id")]
    public string Id { get; set; }
}