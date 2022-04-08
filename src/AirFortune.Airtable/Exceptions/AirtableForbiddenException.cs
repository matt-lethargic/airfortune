using System.Net;

namespace AirFortune.Airtable.Exceptions;

public class AirtableForbiddenException : AirtableApiException
{
    public AirtableForbiddenException() : base(
        HttpStatusCode.Forbidden,
        "Forbidden",
        "Accessing a protected resource with API credentials that don't have access to that resource.")
    {
    }
}