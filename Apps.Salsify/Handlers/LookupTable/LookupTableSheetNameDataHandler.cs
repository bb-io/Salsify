using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers.Asset;
using Apps.Salsify.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salsify.Handlers.LookupTable;

public class LookupTableSheetNameDataHandler : SalsifyInvocable, IAsyncDataSourceItemHandler
{
    private readonly AssetHelper _assetHelper;
    private readonly string _tableId;

    public LookupTableSheetNameDataHandler(InvocationContext context, [ActionParameter] LookupTableIdentifier tableIdentifier) 
        : base(context)
    {
        if (string.IsNullOrWhiteSpace(tableIdentifier.AssetId))
            throw new PluginMisconfigurationException("Please specify a table asset ID first");
        
        _tableId = tableIdentifier.AssetId;
        _assetHelper = new AssetHelper(context);
    }

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var downloadedAsset = await _assetHelper.DownloadAsset(_tableId);

        using var stream = new MemoryStream(downloadedAsset.Bytes);
        var workbook = stream.ToWorkbook();

        var sheetNames = workbook.Worksheets.Select(x => x.Name);
        if (!string.IsNullOrEmpty(context.SearchString))
            sheetNames = sheetNames.Where(x => x.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase));
        
        return sheetNames.Select(x => new DataSourceItem(x, x)).ToList();
    }
}