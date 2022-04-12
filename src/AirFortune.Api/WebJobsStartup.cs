using AirFortune.Api;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(WebJobsStartup))]

namespace AirFortune.Api;

public class WebJobsStartup : IWebJobsStartup
{
    public void Configure(IWebJobsBuilder builder)
    {
        builder.AddExtension<StaticWebAppsPrincipalConfigProvider>();

        builder.Services
            .AddTransient<StaticWebAppsPrincipalBinding>()
            .AddTransient<StaticWebAppsPrincipalBindingProvider>()
            .AddTransient<StaticWebAppsPrincipalValueProvider>();
    }
}