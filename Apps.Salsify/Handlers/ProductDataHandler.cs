using Apps.Salsify.Api;
using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers;
using Apps.Salsify.Models.Entities.Product;
using Apps.Salsify.Models.Responses.Product;
using Apps.Salsify.Models.Responses.Product.Api;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salsify.Handlers;

public class ProductDataHandler(InvocationContext context) : SalsifyInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        string nameProperty = await ProductHelper.GetProductNamePropertyName(Client);

        string? query = null;
        if (!string.IsNullOrEmpty(context.SearchString))
            query = $"='{nameProperty}':contains('{context.SearchString}')";
        
        var request = new SalsifyRequest("products").AddQueryParameterIfNotEmpty("filter", query);
        var response = await Client.PaginateCursor<ListProductsResponse, ProductEntity>(request, paginateTimes: 2);
        var productsWithName = response.Select(x => new ProductResponse(x, nameProperty)).ToArray();

        return productsWithName.Select(x => new DataSourceItem(x.Id, string.IsNullOrWhiteSpace(x.Name) ? x.Id : x.Name));
    }
}