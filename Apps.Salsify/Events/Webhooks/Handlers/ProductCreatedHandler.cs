using Apps.Salsify.Events.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salsify.Events.Webhooks.Handlers;

public class ProductCreatedHandler(InvocationContext invocationContext) : BaseWebhookHandler(invocationContext)
{
    protected override string TriggerType => "add";
    protected override string EntityType => "product";
}