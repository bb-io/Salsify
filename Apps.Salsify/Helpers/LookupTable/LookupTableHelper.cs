using Apps.Salsify.Converters.LookupTable;
using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers.Asset;
using Apps.Salsify.Helpers.LookupTable.Models;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Filters.Coders;

namespace Apps.Salsify.Helpers.LookupTable;

public class LookupTableHelper(InvocationContext invocationContext) : SalsifyInvocable(invocationContext)
{
    private AssetHelper AssetHelper { get; } = new(invocationContext);

    public async Task<TranslatedLookupTable> ApplyTranslation(string tableHtml, string fileName, string? sourceAssetId)
    {
        var cells = LookupTableWorkbookConverter.ParseCells(tableHtml);
        if (cells.Count == 0)
            throw new PluginMisconfigurationException("The file contains no translated cells");

        var coded = new HtmlCoder().Deserialize(tableHtml, fileName);
        string assetId = 
            sourceAssetId ?? 
            coded.SystemReference.ContentId ?? 
            throw new PluginMisconfigurationException("Table asset ID was not found in the file. Please provide it in the input");

        var sourceAsset = await AssetHelper.DownloadAsset(assetId);

        using var stream = new MemoryStream(sourceAsset.Bytes);
        using var workbook = stream.ToWorkbook();

        var writeResult = LookupTableWorkbookConverter.WriteCells(workbook, cells);

        if (writeResult.Written == 0)
        {
            throw new PluginMisconfigurationException(
                $"None of the {cells.Count} translated cells could be written to '{assetId}'. " +
                "The spreadsheet may have changed since it was downloaded");
        }

        if (writeResult.Skipped.Count > 0)
        {
            string skipped = string.Join(", ", writeResult.Skipped);
            string warningMsg = $"Skipped {writeResult.Skipped.Count} cell(s) that no longer exist or now contain formulas: {skipped}";
            InvocationContext.Logger?.LogWarning(warningMsg, []);
        }

        using var outputStream = new MemoryStream();
        workbook.SaveAs(outputStream);

        return new(assetId, outputStream.ToArray(), sourceAsset.Filename);
    }
}