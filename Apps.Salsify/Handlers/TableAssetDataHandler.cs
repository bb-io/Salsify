using Apps.Salsify.Api;
using Apps.Salsify.Models.Entities.Asset;
using Apps.Salsify.Models.Responses.Asset.Api;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Salsify.Handlers;

public class TableAssetDataHandler(InvocationContext context) : SalsifyInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        string query = "='salsify:filename':contains('.xlsx')";
        if (!string.IsNullOrEmpty(context.SearchString))
            query += $",'salsify:name':contains('{context.SearchString}')";
        
        var request = new SalsifyRequest("digital_assets").AddQueryParameter("filter", query);
        var response = await Client.PaginateCursor<ListAssetsResponse, AssetEntity>(request, paginateTimes: 1);

        return response
            .Where(x => x.Filename is not null && x.Filename.EndsWith(".xlsx"))
            .Select(x => new DataSourceItem(x.Id, x.Name ?? x.Filename ?? x.Id))
            .ToList();
    }
}