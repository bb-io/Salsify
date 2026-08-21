using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers.Asset;
using Apps.Salsify.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using ClosedXML.Excel;

namespace Apps.Salsify.Handlers.LookupTable;

public class LookupTableColumnDataHandler : SalsifyInvocable, IAsyncDataSourceItemHandler
{
    private readonly AssetHelper _assetHelper;
    private readonly string _tableId;
    private readonly string _sheetName;

    public LookupTableColumnDataHandler(InvocationContext context,
        [ActionParameter] LookupTableIdentifier tableIdentifier,
        [ActionParameter] LookupTableSheetNameIdentifier sheetIdentifier) : base(context)
    {
        if (string.IsNullOrWhiteSpace(tableIdentifier.AssetId))
            throw new PluginMisconfigurationException("Please specify a table asset ID first");
        
        if (string.IsNullOrWhiteSpace(sheetIdentifier.SheetName))
            throw new PluginMisconfigurationException("Please specify a table sheet name first");
        
        _tableId = tableIdentifier.AssetId;
        _sheetName = sheetIdentifier.SheetName.Trim();
        _assetHelper = new AssetHelper(context);
    }

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var downloadedAsset = await _assetHelper.DownloadAsset(_tableId);

        using var stream = new MemoryStream(downloadedAsset.Bytes);
        var workbook = stream.ToWorkbook();

        var sheet = workbook.Worksheets.FirstOrDefault(x => string.Equals(x.Name, _sheetName, StringComparison.OrdinalIgnoreCase));
        if (sheet is null)
            return [];
        
        var headerRow = sheet.Row(1);
        int? lastColumn = sheet.LastColumnUsed()?.ColumnNumber();
        if (lastColumn is null)
            return [];
        
        var items = new List<DataSourceItem>();
        for (int columnNumber = 1; columnNumber <= lastColumn; columnNumber++)
        {
            string letter = XLHelper.GetColumnLetterFromNumber(columnNumber);
            string header = headerRow.Cell(columnNumber).GetString().Trim();
            string displayName = string.IsNullOrEmpty(header) ? letter : $"{letter} - {header}";

            if (!string.IsNullOrWhiteSpace(context.SearchString) && !displayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                continue;

            items.Add(new DataSourceItem(letter, displayName));
        }

        return items;
    }
}