using Apps.Salsify.Constants.Webhooks;
using Apps.Salsify.Events.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salsify.Events.Webhooks.Handlers;

public class AssetUpdatedHandler(InvocationContext invocationContext) : BaseWebhookHandler(invocationContext)
{
    protected override AlertTriggerType TriggerType => AlertTriggerType.Change;
    protected override AlertEntityType EntityType => AlertEntityType.DigitalAsset;
}