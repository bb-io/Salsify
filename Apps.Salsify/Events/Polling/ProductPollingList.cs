using Apps.Salsify.Api;
using Apps.Salsify.Events.Polling.Models.Memory;
using Apps.Salsify.Events.Polling.Models.Request.Product;
using Apps.Salsify.Helpers.Event;
using Apps.Salsify.Helpers.Query;
using Apps.Salsify.Models.Entities.Product;
using Apps.Salsify.Models.Responses.Product;
using Apps.Salsify.Models.Responses.Product.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Salsify.Events.Polling;

[PollingEventList("Products")]
public class ProductPollingList(InvocationContext invocationContext) : SalsifyInvocable(invocationContext)
{
    [MultipleEvents] 
    [PollingEvent("On product added to list", Description = "Triggered when a product is added to a specific list")]
    public async Task<PollingEventResponse<ProductIdsMemory, List<ProductResponse>>> OnProductAddedToList(
        PollingEventRequest<ProductIdsMemory> pollingRequest,
        [PollingEventParameter] OnProductAddedToListRequest input)
    {
        Filter?[] query = [Filter.InList(input.ProductListId)];
        var productsRequest = new SalsifyRequest("products").AddQueryParameter("filter", Filter.Build(query));
        var products = await Client.PaginateCursor<ListProductsResponse, ProductEntity>(productsRequest);

        var memory = new ProductIdsMemory(products.Select(x => x.SystemId).ToArray());
        if (pollingRequest.Memory is null)
            return PollingResult.DoNotFly<ProductIdsMemory, List<ProductResponse>>(memory);

        var knownIds = pollingRequest.Memory.ProductIds.ToArray();
        var newProducts = products
            .Where(x => !knownIds.Contains(x.SystemId))
            .Select(x => new ProductResponse(x))
            .ToList();

        return newProducts.Count > 0
            ? PollingResult.Fly(memory, newProducts)
            : PollingResult.DoNotFly<ProductIdsMemory, List<ProductResponse>>(memory);
    }
}