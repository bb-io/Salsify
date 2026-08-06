using Apps.Salsify.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Salsify.Events.Webhooks.Handlers;

// Implement IWebhookEventHandler so Blackbird registers the webhook in the third-party system
// when a bird is published and removes it when the bird is deleted.
// One handler per event; the event a handler subscribes to is fixed here (see the topic below).
public class ItemCreatedHandler(InvocationContext invocationContext) : BaseInvocable(invocationContext), IWebhookEventHandler
{
    private const string Topic = "item.created";

    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider,
        Dictionary<string, string> values)
    {
        // values["payloadUrl"] is the callback URL Blackbird generates for this bird.
        // Register it with your API so the third-party system starts POSTing events to it.
        var client = new SalsifyClient(authenticationCredentialsProvider);
        var payload = new
        {
            url = values["payloadUrl"],
            events = new[] { Topic }
        };

        var request = new RestRequest("/webhooks", Method.Post)
            .AddJsonBody(JsonConvert.SerializeObject(payload));
        await client.ExecuteWithErrorHandling(request);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider,
        Dictionary<string, string> values)
    {
        // Find the subscription that matches this bird's payload URL and delete it.
        var client = new SalsifyClient(authenticationCredentialsProvider);
        var listRequest = new RestRequest("/webhooks", Method.Get);
        var response = await client.ExecuteWithErrorHandling(listRequest);

        var webhooks = JsonConvert.DeserializeObject<List<WebhookSubscription>>(response.Content ?? "[]") ?? [];
        var subscription = webhooks.FirstOrDefault(x => x.Url == values["payloadUrl"]);
        if (subscription is null)
            return;

        var deleteRequest = new RestRequest($"/webhooks/{subscription.Id}", Method.Delete);
        await client.ExecuteWithErrorHandling(deleteRequest);
    }

    private class WebhookSubscription
    {
        [JsonProperty("id")] public string Id { get; set; } = string.Empty;
        [JsonProperty("url")] public string Url { get; set; } = string.Empty;
    }
}
