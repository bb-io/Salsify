using Apps.Salsify.Constants.Webhooks;
using Apps.Salsify.Events.Webhooks.Handlers.Base;
using Apps.Salsify.Models.Identifiers.Optional;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Salsify.Events.Webhooks.Handlers;

public class ProductUpdatedHandler(InvocationContext invocationContext, [WebhookParameter] LocaleOptionalIdentifier localeIdentifier) 
    : BaseWebhookHandler(invocationContext)
{
    protected override AlertTriggerType TriggerType => AlertTriggerType.Change;
    protected override AlertEntityType EntityType => AlertEntityType.Product;
    protected override IEnumerable<string?> Locales { get; } = [localeIdentifier.Locale];
}