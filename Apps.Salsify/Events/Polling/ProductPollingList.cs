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
        var products = await GetProductsInList(input.ProductListId);

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


    [MultipleEvents]
    [PollingEvent("On product in list",
        Description = "Triggered on every check with all products currently in the list, whether anything changed")]
    public async Task<PollingEventResponse<EmptyMemory, List<ProductResponse>>> OnProductInList(
        PollingEventRequest<EmptyMemory> pollingRequest,
        [PollingEventParameter] OnProductInListRequest input)
    {
        var emptyMemory = new EmptyMemory();

        if (input.Limit is <= 0)
            throw new ArgumentOutOfRangeException(nameof(input.Limit),
                "Limit must be greater than zero.");

        var products = await GetProductsInList(input.ProductListId);

        if (input.Limit.HasValue)
            products = products.Take(input.Limit.Value).ToList();

        if (products.Count == 0)
            return PollingResult.DoNotFly<EmptyMemory, List<ProductResponse>>(emptyMemory);

        var responses = products.Select(x => new ProductResponse(x)).ToList();
        return PollingResult.Fly(emptyMemory, responses);
    }
    private Task<List<ProductEntity>> GetProductsInList(string listId)
    {
        Filter?[] query = [Filter.InList(listId)];
        var productsRequest = new SalsifyRequest("products").AddQueryParameter("filter", Filter.Build(query));
        return Client.PaginateCursor<ListProductsResponse, ProductEntity>(productsRequest);
    }

}