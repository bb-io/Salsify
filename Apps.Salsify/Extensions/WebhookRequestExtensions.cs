using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;

namespace Apps.Salsify.Extensions;

public static class WebhookRequestExtensions
{
    public static T DeserializePayload<T>(this WebhookRequest request)
    {
        var payload = JsonConvert.DeserializeObject<T>(request.Body.ToString()!);
        return payload ?? throw new InvalidCastException(nameof(request.Body));
    }
}