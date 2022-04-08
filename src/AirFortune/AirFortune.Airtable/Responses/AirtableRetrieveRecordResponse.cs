using AirFortune.Airtable.Exceptions;

namespace AirFortune.Airtable.Responses;

public class AirtableRetrieveRecordResponse : AirtableApiResponse
{
    public AirtableRetrieveRecordResponse(AirtableApiException error) : base(error)
    {
        Record = null;
    }

    public AirtableRetrieveRecordResponse(AirtableRecord? record)
    {
        Record = record;
    }

    public readonly AirtableRecord? Record;
}

public class AirtableRetrieveRecordResponse<T> : AirtableApiResponse
{
    public AirtableRetrieveRecordResponse(AirtableApiException error) : base(error)
    {
        Record = null;
    }

    public AirtableRetrieveRecordResponse(AirtableRecord<T>? record)
    {
        Record = record;
    }

    public readonly AirtableRecord<T>? Record;
}