using Apps.Salsify.Events.Webhooks.Handlers.Base;
using Apps.Salsify.Models.Identifiers;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Salsify.Events.Webhooks.Handlers;

public class ProductUpdatedHandler(InvocationContext invocationContext, [WebhookParameter] LocaleIdentifier localeIdentifier) 
    : BaseWebhookHandler(invocationContext)
{
    protected override string TriggerType => "change";
    protected override string EntityType => "product";
    protected override IEnumerable<string> Locales { get; } = [localeIdentifier.Locale];
}