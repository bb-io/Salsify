using Apps.Salsify.Api;
using Apps.Salsify.Extensions;
using Apps.Salsify.Models.Entities.Asset;
using Apps.Salsify.Models.Responses.Asset.Api;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salsify.Handlers;

public class AssetDataHandler(InvocationContext context) : SalsifyInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        string? query = null;
        if (!string.IsNullOrEmpty(context.SearchString))
            query = $"='salsify:name':contains('{context.SearchString}')";
        
        var request = new SalsifyRequest("digital_assets").AddQueryParameterIfNotEmpty("filter", query);
        var response = await Client.PaginateCursor<ListAssetsResponse, AssetEntity>(request, paginateTimes: 1);

        return response.Select(x => new DataSourceItem(x.Id, x.ToString())).ToList();
    }
}