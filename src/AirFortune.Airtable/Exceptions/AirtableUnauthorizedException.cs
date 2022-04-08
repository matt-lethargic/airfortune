using System.Net;

namespace AirFortune.Airtable.Exceptions;

public class AirtableUnauthorizedException : AirtableApiException
{
    public AirtableUnauthorizedException() : base(HttpStatusCode.Unauthorized, "Unauthorized", "Accessing a protected resource without authorization or with invalid credentials.")
    {
    }
}