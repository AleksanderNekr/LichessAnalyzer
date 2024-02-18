using System.Net;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Serilog;

namespace Backend;

public static class RetryPolicyConfigurator
{
    internal static IHttpClientBuilder AddRetryPolicy(this IHttpClientBuilder clientBuilder)
    {

        return clientBuilder
              .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
                                                    .Or<TimeoutRejectedException>()
                                                    .WaitAndRetryAsync(
                                                         retryCount: 3,
                                                         sleepDurationProvider: i => TimeSpan.FromSeconds(i + 2),
                                                         onRetry: OnTransientRetry))
              .AddPolicyHandler(Policy<HttpResponseMessage>
                               .HandleResult(message => message.StatusCode == HttpStatusCode.TooManyRequests)
                               .WaitAndRetryAsync(
                                    retryCount: 1,
                                    _ => TimeSpan.FromMinutes(1),
                                    onRetry: OnTooManyRetry));
    }

    private static void OnTransientRetry(DelegateResult<HttpResponseMessage> result, TimeSpan span)
    {
        Log.Warning("Retrying {@Result} in {Span}", result, span);
    }

    private static void OnTooManyRetry(DelegateResult<HttpResponseMessage> result, TimeSpan span)
    {
        Log.Warning("Retrying 429 code {@Result} in {Span}", result, span);
    }
}
