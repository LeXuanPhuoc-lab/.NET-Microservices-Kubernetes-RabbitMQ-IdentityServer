using System.Net;
using Polly;
using Polly.Extensions.Http;

namespace SearchService.Extensions
{
    public static class HttpClientExtension
    {
        public static IAsyncPolicy<HttpResponseMessage> GetPolicy()
            => HttpPolicyExtensions // From policy extensions
            .HandleTransientHttpError() // Handle Connection Refused 
            .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound) // And also reponse of 404 
            .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3)); // Wait and try to repeat request each 3s
    }
}