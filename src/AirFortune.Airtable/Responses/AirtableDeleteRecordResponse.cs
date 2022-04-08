using AirFortune.Airtable.Exceptions;

namespace AirFortune.Airtable.Responses;

public class AirtableDeleteRecordResponse : AirtableApiResponse
{
    public AirtableDeleteRecordResponse(AirtableApiException error) : base(error)
    {
        Deleted = false;
        Id = null;
    }

    public AirtableDeleteRecordResponse(bool deleted, string? id)
    {
        Deleted = deleted;
        Id = id;
    }

    public readonly bool Deleted;
    public readonly string? Id;
}