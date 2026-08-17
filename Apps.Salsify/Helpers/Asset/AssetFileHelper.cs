using Apps.Salsify.Api;
using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers.Asset.Models;
using Apps.Salsify.Models.Entities.Asset;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Salsify.Helpers.Asset;

public class AssetFileHelper(InvocationContext context) : SalsifyInvocable(context)
{
    public async Task<DownloadedAsset> DownloadAsset(string assetId)
    {
        var request = new SalsifyRequest($"digital_assets/{assetId}");
        var asset = await Client.ExecuteWithErrorHandling<AssetEntity>(request);

        if (string.IsNullOrWhiteSpace(asset.Url))
            throw new PluginMisconfigurationException($"Asset '{assetId}' does not have a download URL. Asset status - {asset.Status}");

        var downloadRequest = new RestRequest(asset.Url);
        var downloadResponse = await ExternalClient.ExecuteWithErrorHandling(downloadRequest);
        if (downloadResponse.RawBytes is null or { Length: 0 })
            throw new PluginApplicationException($"Failed to download asset '{assetId}' - file has no content");

        string assetFileName = asset.Filename!;
        string assetName = (asset.Name ?? assetFileName).RemoveFromEnd(".xls", ".xlsx");
        string contentType = downloadResponse.ContentType ?? "application/octet-stream";
        
        return new(downloadResponse.RawBytes, assetFileName, assetName, contentType);
    }
}