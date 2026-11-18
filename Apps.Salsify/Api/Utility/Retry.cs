using System.Net;
using Polly;
using Polly.Retry;
using RestSharp;

namespace Apps.Salsify.Api.Utility;

public static class Retry
{
    private static readonly HashSet<HttpStatusCode> TransientStatusCodes =
    [
        HttpStatusCode.RequestTimeout,
        HttpStatusCode.TooManyRequests,
        HttpStatusCode.BadGateway,
        HttpStatusCode.ServiceUnavailable,
        HttpStatusCode.GatewayTimeout
    ];

    public static readonly ResiliencePipeline<RestResponse> Pipeline = new ResiliencePipelineBuilder<RestResponse>()
        .AddRetry(new RetryStrategyOptions<RestResponse>
        {
            ShouldHandle = new PredicateBuilder<RestResponse>()
                .HandleResult(x => x.Request.Method is Method.Get && 
                                   (x.ResponseStatus == ResponseStatus.TimedOut || TransientStatusCodes.Contains(x.StatusCode))),
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(1),
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
        })
        .Build();
}