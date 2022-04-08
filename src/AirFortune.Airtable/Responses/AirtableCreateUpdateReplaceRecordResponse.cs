using AirFortune.Airtable.Exceptions;

namespace AirFortune.Airtable.Responses;

public class AirtableCreateUpdateReplaceRecordResponse : AirtableApiResponse
{
    public AirtableCreateUpdateReplaceRecordResponse(AirtableApiException error) : base(error)
    {
        Record = null;
    }

    public AirtableCreateUpdateReplaceRecordResponse(AirtableRecord? record)
    {
        Record = record;
    }

    public readonly AirtableRecord? Record;
}