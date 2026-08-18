using System.Net;
using Apps.Salsify.Constants;
using Apps.Salsify.Events.Webhooks.Handlers;
using Apps.Salsify.Events.Webhooks.Models.Payloads;
using Apps.Salsify.Extensions;
using Apps.Salsify.Models.Responses;
using Apps.Salsify.Models.Responses.Product;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Salsify.Events.Webhooks;

[WebhookList("Products")]
public class ProductWebhookList(InvocationContext invocationContext) : SalsifyInvocable(invocationContext)
{
    [Webhook("On product updated", typeof(ProductUpdatedHandler), Description = "Triggered when a product is updated")]
    public async Task<WebhookResponse<SearchProductsResponse>> OnProductUpdated(WebhookRequest webhookRequest)
    {
        var payload = webhookRequest.DeserializePayload<ProductWebhookPayload>();
        
        var current = await Client.GetCurrentOrgInfo();
        string productNameProperty = current.GetRolePropertyId(RolePropertyNames.ProductName) ?? string.Empty;

        var products = payload.Products.Select(x => new ProductResponse(x, productNameProperty)).ToArray();
        return new WebhookResponse<SearchProductsResponse>
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            Result = new(products)
        };
    }
    
    [Webhook("On product created", typeof(ProductCreatedHandler), Description = "Triggered when a product is created")]
    public async Task<WebhookResponse<SearchProductsResponse>> OnProductCreated(WebhookRequest webhookRequest)
    {
        var payload = webhookRequest.DeserializePayload<ProductWebhookPayload>();
        
        var current = await Client.GetCurrentOrgInfo();
        string productNameProperty = current.GetRolePropertyId(RolePropertyNames.ProductName) ?? string.Empty;

        var products = payload.Products.Select(x => new ProductResponse(x, productNameProperty)).ToArray();
        return new WebhookResponse<SearchProductsResponse>
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            Result = new(products)
        };
    }

    private static Task<WebhookResponse<ItemResponse>> Preflight() =>
        Task.FromResult(new WebhookResponse<ItemResponse>
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            Result = null,
            ReceivedWebhookRequestType = WebhookRequestType.Preflight
        });
}