using System.Net.Mime;
using Apps.Salsify.Constants;
using Apps.Salsify.Converters.LookupTable;
using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers.Asset;
using Apps.Salsify.Helpers.LookupTable;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Requests.LookupTable;
using Apps.Salsify.Models.Responses.Asset;
using Apps.Salsify.Models.Responses.File;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Coders;

namespace Apps.Salsify.Actions;

[ActionList("Lookup tables")]
public class LookupTableActions(InvocationContext context, IFileManagementClient fileManagementClient) : SalsifyInvocable(context)
{
    [Action("Download lookup table", Description = "Download lookup table file as HTML")]
    public async Task<FileResponse> DownloadLookupTable(
        [ActionParameter] LookupTableIdentifier tableIdentifier,
        [ActionParameter] DownloadLookupTableRequest downloadInput)
    {
        downloadInput.Validate();
        
        var assetHelper = new AssetHelper(InvocationContext);
        var downloadedAsset = await assetHelper.DownloadAsset(tableIdentifier.AssetId);

        using var stream = new MemoryStream(downloadedAsset.Bytes);
        var workbook = stream.ToWorkbook();
        int firstRow = downloadInput.FirstRow ?? 2;
        
        var htmlDoc = LookupTableHtmlConverter.GenerateHtml(workbook, downloadInput.SheetName, downloadInput.ColumnLetters, firstRow);
        
        string fileName = $"{downloadedAsset.AssetName}_{downloadInput.SheetName}.html";
        var coded = new HtmlCoder().Deserialize(htmlDoc.DocumentNode.OuterHtml, fileName);
        coded.SystemReference.ContentId = tableIdentifier.AssetId;
        
        var outputFile = await fileManagementClient.UploadAsync(coded.ToStream(), MediaTypeNames.Text.Html, fileName);
        return new(outputFile);
    }

    [Action("Upload lookup table", Description = "Create a new lookup table from a file")]
    public async Task<AssetResponse> UploadLookupTable([ActionParameter] UploadLookupTableRequest uploadInput)
    {
        await using var fileStream = await fileManagementClient.DownloadAsync(uploadInput.Content);
        string html = await fileStream.ToHtmlString(uploadInput.Content.Name);

        var tableHelper = new LookupTableHelper(InvocationContext);
        var table = await tableHelper.ApplyTranslation(html, uploadInput.Content.Name, uploadInput.SourceAssetId);

        string fileName = table.ResolveFileName(uploadInput.Name);
        var assetHelper = new AssetHelper(InvocationContext);
        var createdAsset = await assetHelper.UploadAsset(table.Bytes, fileName, SpreadsheetMediaTypes.Xlsx, uploadInput.ListName);
        return new(createdAsset);
    }
}