using Apps.Salsify.Events.Webhooks.Handlers;
using Apps.Salsify.Events.Webhooks.Models.Payloads;
using Apps.Salsify.Events.Webhooks.Models.Request.Asset;
using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers;
using Apps.Salsify.Models.Responses.Asset;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Salsify.Events.Webhooks;

[WebhookList("Asset")]
public class AssetWebhookList(InvocationContext invocationContext) : SalsifyInvocable(invocationContext)
{
    [Webhook("On asset updated", typeof(AssetUpdatedHandler), Description = "Triggered when a digital asset is updated")]
    public Task<WebhookResponse<SearchAssetsResponse>> OnAssetUpdated(WebhookRequest webhookRequest,
        [WebhookParameter] OnAssetUpdatedRequest input)
    {
        var payload = webhookRequest.DeserializePayload<AssetWebhookPayload>();
        var assets = payload.Assets;
        
        if (input.AssetIds is { Count: > 0 })
        {
            var requestedIds = input.AssetIds.ToHashSet(StringComparer.Ordinal);
            var matching = assets.Where(x => requestedIds.Contains(x.Id)).ToList();

            if (matching.Count == 0)
                return WebhookResult.Preflight<SearchAssetsResponse>();

            assets = matching;
        }

        var result = new SearchAssetsResponse(assets.Select(x => new AssetResponse(x)).ToArray());
        return WebhookResult.Success(result);
    }
    
    [Webhook("On asset created", typeof(AssetCreatedHandler), Description = "Triggered when a digital asset is created")]
    public Task<WebhookResponse<SearchAssetsResponse>> OnAssetCreated(WebhookRequest webhookRequest)
    {
        var payload = webhookRequest.DeserializePayload<AssetWebhookPayload>();
        
        var result = new SearchAssetsResponse(payload.Assets.Select(x => new AssetResponse(x)).ToArray());
        return WebhookResult.Success(result);
    }
}