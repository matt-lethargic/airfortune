using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;

namespace AirFortune.Api
{
    public class MessageFunction
    {
        private readonly HttpClient _client;

        public MessageFunction(HttpClient client)
        {
            _client = client;
        }


        [FunctionName("message")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "message")] HttpRequest req,
            ILogger log,
            [StaticWebAppsPrincipal] ClaimsPrincipal principal)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (principal is not { Identity.IsAuthenticated: true })
                return new UnauthorizedResult();

            var response = await _client.GetAsync("https://quotes.rest/qod");
            if (!response.IsSuccessStatusCode)
                return new OkObjectResult("Out of quotes for another hour! :(");

            var content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content);
            string quote = json.contents.quotes[0].quote;
            return new OkObjectResult(quote);
        }
    }
}
