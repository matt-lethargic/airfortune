using System.Net;

namespace AirFortune.Airtable.Exceptions;

public class AirtableTooManyRequestsException : AirtableApiException
{
    public AirtableTooManyRequestsException() : base(
        (HttpStatusCode)429,
        "Too Many Requests",
        "The user has sent too many requests in a given amount of time (rate limiting).")
    {
    }
}