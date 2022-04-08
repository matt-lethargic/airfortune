using AirFortune.Airtable.Exceptions;

namespace AirFortune.Airtable.Responses;

public class AirtableCreateUpdateReplaceMultipleRecordsResponse : AirtableApiResponse
{
    public AirtableCreateUpdateReplaceMultipleRecordsResponse(AirtableApiException? error) : base(error)
    {
        Records = null;
    }

    public AirtableCreateUpdateReplaceMultipleRecordsResponse(AirtableRecord[]? records)
    {
        Records = records;
    }

    public readonly AirtableRecord[]? Records;
}