using System.Net;

namespace AirFortune.Airtable.Exceptions;

public class AirtableUnrecognizedException : AirtableApiException
{
    public AirtableUnrecognizedException(HttpStatusCode statusCode) : base(statusCode, "Unrecognized Error", $"Airtable returned HTTP status code {statusCode}")
    {
    }
}