using System.Net;

namespace AirFortune.Airtable.Exceptions;

public class AirtableRequestEntityTooLargeException : AirtableApiException
{
    public AirtableRequestEntityTooLargeException() : base(
        HttpStatusCode.RequestEntityTooLarge,
        "Request Entity Too Large",
        "The request exceeded the maximum allowed payload size. You shouldn't encounter this under normal use.")
    {
    }
}