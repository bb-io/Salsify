using Apps.Salsify.Api;
using Apps.Salsify.Extensions;
using Apps.Salsify.Models.Entities.Product;
using Apps.Salsify.Models.Requests.Product;
using Apps.Salsify.Models.Responses.Product;
using Apps.Salsify.Models.Responses.Product.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Salsify.Actions;

[ActionList("Products")]
public class ProductActions(InvocationContext context) : SalsifyInvocable(context)
{
    [Action("Search products", Description = "Search products")]
    public async Task<SearchProductsResponse> SearchProducts([ActionParameter] SearchProductsRequest searchInput)
    {
        var response = await Client.PaginateCursor<ListProductsResponse, ProductEntity>(cursor => 
            new SalsifyRequest("products")
                .AddQueryParameterIfNotEmpty("query", searchInput.Query)
                .AddQueryParameterIfNotEmpty("cursor", cursor)
                .AddQueryParameter("per_page", "100"));

        var result = response.Select(x => new ProductResponse(x)).ToArray();
        return new(result);
    }
}