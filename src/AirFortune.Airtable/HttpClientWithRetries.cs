using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace AirFortune.Airtable;

internal class HttpClientWithRetries : IDisposable
{
    public const int MaxRetries = 3;
    public const int MinRetryDelayMillisecondsIfRateLimited = 2000;   // 2 second
    public bool ShouldNotRetryIfRateLimited { get; set; }

    private int _retryDelayMillisecondsIfRateLimited;
    public int RetryDelayMillisecondsIfRateLimited
    {
        get => _retryDelayMillisecondsIfRateLimited;
        set
        {
            if (value < MinRetryDelayMillisecondsIfRateLimited)
            {
                throw new ArgumentException($"Retry Delay cannot be less than {MinRetryDelayMillisecondsIfRateLimited} ms.");
            }
            _retryDelayMillisecondsIfRateLimited = value;
        }
    }

    private readonly HttpClient _client;


    public HttpClientWithRetries(DelegatingHandler? delegatingHandler, string apiKey)
    {
        ShouldNotRetryIfRateLimited = false;
        RetryDelayMillisecondsIfRateLimited = MinRetryDelayMillisecondsIfRateLimited;

        _client = delegatingHandler == null 
            ? new HttpClient() 
            : new HttpClient(delegatingHandler);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    /// <summary>
    /// This method has preforms retries with exponential back off if the generic 
    /// SendAsync returns a HttpStatusCode of 429 for Too Many Request.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>HttpStatusCode of 429 for Too Many Request</returns>
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        string? content = null;

        if (request.Content != null)
        {
            content = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        int dueTimeDelay = RetryDelayMillisecondsIfRateLimited;
        int retries = 0;

        HttpResponseMessage response = await _client.SendAsync(request).ConfigureAwait(false);

        while (response.StatusCode == (HttpStatusCode)429 &&
               retries < MaxRetries &&
               !ShouldNotRetryIfRateLimited)
        {
            await Task.Delay(dueTimeDelay).ConfigureAwait(false);
            var requestRegenerated = RegenerateRequest(request.Method, request.RequestUri, content);
            response = await _client.SendAsync(requestRegenerated).ConfigureAwait(false);
            retries++;
            dueTimeDelay *= 2;
        }

        return response;
    }

    /// <summary>
    /// A new HttpRequestMessage needs to be generated for each retry because the same message cannot be used more than once.
    /// </summary>
    /// <returns></returns>
    private HttpRequestMessage RegenerateRequest(HttpMethod method, Uri? requestUri, string? content)
    {
        var request = new HttpRequestMessage(method, requestUri);
        if (content != null)
        {
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");
        }
        return request;
    }

}