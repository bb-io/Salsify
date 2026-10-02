using Apps.Salsify.Api;
using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers.Query;
using Apps.Salsify.Models.Entities.Asset;
using Apps.Salsify.Models.Responses.Asset.Api;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Salsify.Handlers.LookupTable;

public class LookupTableDataHandler(InvocationContext context) : SalsifyInvocable(context), IAsyncDataSourceItemHandler
{
    // Filename does not guarantee the file extension will be preserved.
    // Some files can have no extension in the filename, but they are actually Excel files.
    // The 'salsify:url' property will always have the original file name, so the extension is resolvable from there,
    // but it does not support server-side filtering :(
    //
    // So the behavior is as follows:
    //  - no search string -> paginate without any filtering whatsoever
    //  - search string present -> paginate and filter by name server-side and by url extension client-side
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var queryList = new[] { Filter.Contains("salsify:name", context.SearchString) };
        
        var request = new SalsifyRequest("digital_assets").AddQueryParameter("filter", Filter.Build(queryList));
        var response = await Client.PaginateCursor<ListAssetsResponse, AssetEntity>(request, paginateTimes: 3);
        
        return response
            .Where(x => string.IsNullOrWhiteSpace(context.SearchString) || x.Url is not null && x.Url.IsSpreadsheetUrl())
            .Select(x => new DataSourceItem(x.Id, x.Name ?? x.Filename ?? x.Id))
            .ToList();
    }
}