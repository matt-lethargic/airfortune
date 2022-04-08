using System.Net;

namespace AirFortune.Airtable.Exceptions;

public class AirtableBadRequestException : AirtableApiException
{
    public AirtableBadRequestException() : base(HttpStatusCode.BadRequest, "Bad Request", "The request encoding is invalid; the request can't be parsed as a valid JSON.")
    {
    }
}