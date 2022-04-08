using AirFortune.Airtable.Exceptions;

namespace AirFortune.Airtable.Responses;

public abstract class AirtableApiResponse
{
    protected AirtableApiResponse()
    {
        Success = true;
        AirtableApiError = null;
    }

    protected AirtableApiResponse(AirtableApiException? error)
    {
        Success = false;
        AirtableApiError = error;
    }

    public readonly bool Success;
    public readonly AirtableApiException? AirtableApiError;
}