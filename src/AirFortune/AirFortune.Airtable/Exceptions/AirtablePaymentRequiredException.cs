using System.Net;

namespace AirFortune.Airtable.Exceptions;

public class AirtablePaymentRequiredException : AirtableApiException
{
    public AirtablePaymentRequiredException() : base(
        HttpStatusCode.PaymentRequired,
        "Payment Required",
        "The account associated with the API key making requests hits a quota that can be increased by upgrading the Airtable account plan.")
    {
    }
}