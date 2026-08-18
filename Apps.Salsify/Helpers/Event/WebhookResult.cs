using System.Net;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Salsify.Helpers.Event;

public static class WebhookResult
{
    public static Task<WebhookResponse<T>> Preflight<T>() 
        where T : class
    {
        return Task.FromResult(new WebhookResponse<T>
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            Result = null,
            ReceivedWebhookRequestType = WebhookRequestType.Preflight
        });
    }

    public static Task<WebhookResponse<T>> Success<T>(T result)
        where T : class
    {
        return Task.FromResult(new WebhookResponse<T>
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            Result = result
        });
    }
}