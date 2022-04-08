using System.Net;

namespace AirFortune.Airtable.Exceptions;

public class AirtableInvalidRequestException : AirtableApiException
{
    public string? DetailedErrorMessage { get; }

    public AirtableInvalidRequestException(string? errorMessage = null) : base(
        (HttpStatusCode)422,
        "Invalid Request",
        "The request data is invalid. This includes most of the base-specific validations. The DetailedErrorMessage property contains the detailed error message string.")
    {
        DetailedErrorMessage = errorMessage;
    }
}