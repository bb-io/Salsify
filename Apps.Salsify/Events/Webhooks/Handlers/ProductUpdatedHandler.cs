using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Events.Webhooks.Models;
using Apps.Salsify.Models.Entities.Alert;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Salsify.Events.Webhooks.Handlers;

public class ProductUpdatedHandler(InvocationContext invocationContext) : BaseInvocable(invocationContext), IWebhookEventHandler
{
    private const string TriggerType = "change";
    
    public Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds, Dictionary<string, string> values)
    {
        var client = new SalsifyClient(creds);
        var payload = new
        {
            data = new
            {
                name = $"Blackbird-ProductChange-{Guid.NewGuid()}",
                entity_type = "product",
                filter = "=",
                trigger_type = TriggerType,
                change_type = "any",
                change_property_locale_selection = new
                {
                    type = "any"
                },
                webhook_url = values["payloadUrl"],
                broken = false,
                activated = true,  // Does not make a difference
                include_inherited_property_value_changes = true,
                change_property_ids = Array.Empty<string>()
            }
        };

        var request = new SalsifyRequest("alerts", Method.Post, ApiVersion.Unversioned).WithJsonBody(payload);
        return client.ExecuteWithErrorHandling(request);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds, Dictionary<string, string> values)
    {
        var client = new SalsifyClient(creds);
        
        var alertsRequest = new SalsifyRequest("alerts", Method.Get, ApiVersion.Unversioned);
        var alertsResponse = await client.PaginateOffset<ListAlertsResponse, AlertEntity>(alertsRequest);

        foreach (var subscription in alertsResponse.Where(x => x.WebhookUrl == values["payloadUrl"]))
        {
            var deleteRequest = new SalsifyRequest($"alerts/{subscription.Id}", Method.Delete, ApiVersion.Unversioned)
                .AddQueryParameter("id_type", "system");
            await client.ExecuteWithErrorHandling(deleteRequest);
        }
    }
}