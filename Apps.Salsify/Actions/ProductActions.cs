using Apps.Salsify.Api;
using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers;
using Apps.Salsify.Models.Entities.Product;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Requests.Product;
using Apps.Salsify.Models.Responses.Product;
using Apps.Salsify.Models.Responses.Product.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salsify.Actions;

[ActionList("Products")]
public class ProductActions(InvocationContext context) : SalsifyInvocable(context)
{
    // https://developers.salsify.com/reference/bulk-read-products
    [Action("Search products", Description = "Search products")]
    public async Task<SearchProductsResponse> SearchProducts([ActionParameter] SearchProductsRequest searchInput)
    {
        searchInput.Validate();
        
        string nameProperty = await ProductHelper.GetProductNamePropertyName(Client);

        var queryList = new List<string>();
        
        if (searchInput.UpdatedAfter.HasValue)
            queryList.Add($"'salsify:updated_at':gte('{searchInput.UpdatedAfter.Value.ToSalsifyStringDate()}')");
        
        if (searchInput.UpdatedBefore.HasValue)
            queryList.Add($"'salsify:updated_at':lte('{searchInput.UpdatedBefore.Value.ToSalsifyStringDate()}')");
        
        if (!string.IsNullOrEmpty(searchInput.NameContains))
            queryList.Add($"'{nameProperty}':contains('{searchInput.NameContains}')");
        
        if (!string.IsNullOrEmpty(searchInput.Query))
            queryList.Add(searchInput.Query.TrimStart('='));

        string? query = null;
        if (queryList.Count != 0)
            query = "=" + string.Join(',', queryList);
        
        var productsRequest = new SalsifyRequest("products").AddQueryParameterIfNotEmpty("filter", query);
        var productsResponse = await Client.PaginateCursor<ListProductsResponse, ProductEntity>(productsRequest);

        var result = productsResponse.Select(x => new ProductResponse(x, nameProperty)).ToArray();
        return new(result);
    }

    // https://developers.salsify.com/reference/read-product-record
    [Action("Get product", Description = "Get details for a specific product")]
    public async Task<ProductResponse> GetProduct([ActionParameter] ProductIdentifier productIdentifier)
    {
        string nameProperty = await ProductHelper.GetProductNamePropertyName(Client);
        var request = new SalsifyRequest($"products/{productIdentifier.ProductId}");
        var response = await Client.ExecuteWithErrorHandling<ProductEntity>(request);
        return new(response, nameProperty);
    }
}