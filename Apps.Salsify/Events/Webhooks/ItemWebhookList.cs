using System.Net;
using Apps.Salsify.Events.Webhooks.Handlers;
using Apps.Salsify.Models.Requests;
using Apps.Salsify.Models.Responses;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;

namespace Apps.Salsify.Events.Webhooks;

[WebhookList("Items")]
public class ItemWebhookList(InvocationContext invocationContext) : SalsifyInvocable(invocationContext)
{
    // The handler type (second argument) owns subscription/unsubscription; this method handles the incoming request.
    [BlueprintEventDefinition(BlueprintEvent.ContentCreatedOrUpdated)]
    [Webhook("On item created", typeof(ItemCreatedHandler),
        Description = "Triggered when a content item is created in the third-party system.")]
    public Task<WebhookResponse<ItemResponse>> OnItemCreated(WebhookRequest webhookRequest,
        [WebhookParameter] ItemWebhookInput input)
    {
        var payload = JsonConvert.DeserializeObject<ItemWebhookPayload>(webhookRequest.Body.ToString()!);
        if (payload is null)
            throw new InvalidCastException(nameof(webhookRequest.Body));

        // Preflight: acknowledge the request (HTTP 200) but don't trigger the bird when it doesn't match the filter.
        if (input.ContentId is not null && input.ContentId != payload.Id)
            return Preflight();

        // Your real implementation to map the payload (and optionally enrich it via the API) goes here.
        return Task.FromResult(new WebhookResponse<ItemResponse>
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            Result = new ItemResponse { ContentId = payload.Id }
        });
    }

    private static Task<WebhookResponse<ItemResponse>> Preflight() =>
        Task.FromResult(new WebhookResponse<ItemResponse>
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            Result = null,
            ReceivedWebhookRequestType = WebhookRequestType.Preflight
        });
}
