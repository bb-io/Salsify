using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Extensions;
using Apps.Salsify.Models.Entities.List;
using Apps.Salsify.Models.Responses.List.Api;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Salsify.Handlers;

public class ProductListIdDataHandler(InvocationContext context) : SalsifyInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new SalsifyRequest("lists", Method.Get, ApiVersion.Unversioned)
            .AddQueryParameter("entity_type", "product")
            .AddQueryParameterIfNotEmpty("query", context.SearchString);

        var response = await Client.PaginateOffset<ListListsResponse, ListEntity>(request, paginateTimes: 2);
        return response.Select(x => new DataSourceItem(x.Id, x.Name)).ToList();
    }
}