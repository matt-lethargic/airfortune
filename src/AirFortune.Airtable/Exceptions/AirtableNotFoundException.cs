using System.Net;

namespace AirFortune.Airtable.Exceptions;

public class AirtableNotFoundException : AirtableApiException
{
    public AirtableNotFoundException() : base(
        HttpStatusCode.NotFound,
        "Not Found",
        "Route or resource is not found. This error is returned when the request hits an undefined route, or if the resource doesn't exist (e.g. has been deleted).")
    {
    }
}