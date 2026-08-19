using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Extensions;
using Apps.Salsify.Models.Entities.Asset;
using Apps.Salsify.Models.Responses.Asset.Api;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Salsify.Handlers;

public class AssetListNameDataHandler(InvocationContext context) : SalsifyInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new SalsifyRequest("lists", Method.Get, ApiVersion.Unversioned)
            .AddQueryParameter("entity_type", "digital_asset")
            .AddQueryParameter("type", "simple")
            .AddQueryParameterIfNotEmpty("query", context.SearchString);

        var response = await Client.PaginateOffset<ListAssetListsResponse, AssetListEntity>(request, paginateTimes: 2);
        return response.Select(x => new DataSourceItem(x.Name, x.Name)).ToList();
    }
}