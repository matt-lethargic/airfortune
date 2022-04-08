using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirFortune.Airtable.Responses;

public class AirtableRecord<T> : AirtableRecordBase
{
    [JsonPropertyName("fields")]
    [JsonInclude]
    public T? Fields { get; internal set; }
}

public class AirtableRecord : AirtableRecordBase
{
    [JsonPropertyName("fields")]
    [JsonInclude]
    public Dictionary<string, object?> Fields { get; internal set; } = new();

    public object? GetField(string fieldName)
    {
        if (Fields.ContainsKey(fieldName))
        {
            return Fields[fieldName];
        }
        return null;
    }

    /// <summary>
    /// This method does not communicate with Airtable.
    /// It only helps the user to entangle an Attachments field given the name of the Attachments field.
    /// Special care is taken to make sure the field for the input argument is actually has attachments.
    /// </summary>
    /// <param name="attachmentsFieldName"></param>
    /// <returns><see cref="IEnumerable{AirtableAttachment}"/></returns>
    /// <remarks>If there is no such field or there are no attachments in this field, it will return null.</remarks>
    /// <exception cref="ArgumentException"></exception>
    public IEnumerable<AirtableAttachment>? GetAttachmentField(string attachmentsFieldName)
    {
        var attachmentField = GetField(attachmentsFieldName);
        if (attachmentField == null)
        {
            return null;
        }

        var attachments = new List<AirtableAttachment?>();
        try
        {
            var json = JsonSerializer.Serialize(attachmentField);
            var rawAttachments = JsonSerializer.Deserialize<IEnumerable<Dictionary<string, object>>>(json);

            if (rawAttachments != null)
                foreach (var rawAttachment in rawAttachments)
                {
                    json = JsonSerializer.Serialize(rawAttachment);
                    attachments.Add(JsonSerializer.Deserialize<AirtableAttachment>(json));
                }
        }
        catch (Exception error)
        {
            throw new ArgumentException($"Field '{attachmentsFieldName}' is not an Attachments field.", error);
        }
        return attachments;
    }
}