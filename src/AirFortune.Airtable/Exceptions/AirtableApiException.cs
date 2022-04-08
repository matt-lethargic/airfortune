using System.Net;

namespace AirFortune.Airtable.Exceptions;


public abstract class AirtableApiException : Exception
{
    protected AirtableApiException(HttpStatusCode errorCode, string errorName, string errorMessage) : base($"{errorName} - {errorCode}: {errorMessage}")
    {
        ErrorCode = errorCode;
        ErrorName = errorName;
        ErrorMessage = errorMessage;
    }

    public readonly HttpStatusCode ErrorCode;
    public readonly string ErrorName;
    public readonly string ErrorMessage;
}