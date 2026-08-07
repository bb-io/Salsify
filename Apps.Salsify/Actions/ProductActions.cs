using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Extensions;
using Apps.Salsify.Models.Entities.Product;
using Apps.Salsify.Models.Requests.Product;
using Apps.Salsify.Models.Responses.Product;
using Apps.Salsify.Models.Responses.Product.Api;
using Apps.Salsify.Models.Utility.Current;
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
        var currentRequest = new SalsifyRequest("current", Method.Get, ApiVersion.Internal);
        var currentResponse = await Client.ExecuteWithErrorHandling<CurrentResponse>(currentRequest);
        string nameProperty = currentResponse.RoleProperties.First(x => x.Role == "product_name").ExternalId;
        
        var response = await Client.PaginateCursor<ListProductsResponse, ProductEntity>(cursor => 
            new SalsifyRequest("products")
                .AddQueryParameterIfNotEmpty("query", searchInput.Query)
                .AddQueryParameterIfNotEmpty("cursor", cursor)
                .AddQueryParameter("per_page", "100"));

        var result = response.Select(x => new ProductResponse(x, nameProperty)).ToArray();
        return new(result);
    }
}