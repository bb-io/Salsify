using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers.Asset.Models;
using Apps.Salsify.Models.Entities.Asset;
using Apps.Salsify.Models.Responses.Asset.Api;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Salsify.Helpers.Asset;

public class AssetHelper(InvocationContext context) : SalsifyInvocable(context)
{
    public Task<AssetEntity> GetAsset(string assetId)
    {
        var request = new SalsifyRequest($"digital_assets/{assetId}");
        return Client.ExecuteWithErrorHandling<AssetEntity>(request);
    }

    public Task RenameAsset(string assetId, string newName)
    {
        var updateNameBody = new Dictionary<string, string> { { "salsify:name", newName } };
        var updateNameRequest = new SalsifyRequest($"digital_assets/{assetId}", Method.Put).WithJsonBody(updateNameBody);
        return Client.ExecuteWithErrorHandling(updateNameRequest);
    }
    
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

    public async Task<AssetEntity> UploadAsset(byte[] fileBytes, string fileName, string contentType, string listName)
    {
        var mountBody = new Dictionary<string, object>
        {
            ["list_name"] = listName,
            ["asset_count"] = 1,
            ["property_values"] = Array.Empty<string>(),
            ["source_urls"] = Array.Empty<string>(),
            ["status"] = new { total = 0, uploaded = 0, errors = Array.Empty<string>() }
        };

        var mountRequest = new SalsifyRequest("digital_asset_uploads", Method.Post, ApiVersion.Unversioned).WithJsonBody(mountBody);
        var mount = await Client.ExecuteWithErrorHandling<MountAssetResponse>(mountRequest);

        // The list may already hold assets from a previous run, so snapshot it before uploading
        // and diff afterwards - that is the only way to identify the asset we just created
        var knownAssets = await ListAssets(mount.Upload.List.Filter);
        var knownAssetIds = knownAssets.Select(x => x.Id).ToHashSet(StringComparer.Ordinal);

        await UploadToMount(mount.Mount, fileBytes, fileName, contentType);

        // Cloudinary notifies Salsify separately via the notification_url in the signed payload,
        // which is what actually creates the asset
        var finalizeRequest = new SalsifyRequest($"digital_asset_uploads/{mount.Upload.Id}", Method.Put, ApiVersion.Unversioned)
            .WithJsonBody(new
            {
                status = new
                {
                    total = 1, 
                    uploaded = 1, 
                    errors = Array.Empty<string>()
                }
            });
        await Client.ExecuteWithErrorHandling(finalizeRequest);

        var asset = await AwaitCreatedAsset(mount.Upload.List.Filter, knownAssetIds);
        return asset;
    }

    public async Task ReplaceAsset(byte[] fileBytes, string assetId, string fileName, string contentType)
    {
        var mountRequest = new SalsifyRequest("digital_assets/mounts", Method.Post, ApiVersion.Unversioned);
        var mount = await Client.ExecuteWithErrorHandling<MountResponse>(mountRequest);
        
        var uploadResponse = await UploadToMount(mount, fileBytes, fileName, contentType);
        
        var replaceRequest = new SalsifyRequest($"digital_assets/{assetId}", Method.Put, ApiVersion.Unversioned)
            .WithJsonBody(new
            {
                data = new
                {
                    id = assetId,
                    upload_response_attributes = uploadResponse
                }
            });

        await Client.ExecuteWithErrorHandling(replaceRequest);
    }
    
    private async Task<AssetEntity> AwaitCreatedAsset(string listFilter, HashSet<string> knownAssetIds)
    {
        for (int attempt = 0; attempt < 15; attempt++)
        {
            await Task.Delay(1500);

            var assets = await ListAssets(listFilter);
            var created = assets.FirstOrDefault(x => !knownAssetIds.Contains(x.Id));
            if (created is not null) 
                return created;
        }

        throw new PluginApplicationException(
            "The file was uploaded but Salsify did not finish processing it in time. " +
            "It may still appear in the digital assets library shortly");
    }
    
    private Task<List<AssetEntity>> ListAssets(string filter)
    {
        var request = new SalsifyRequest("digital_assets").AddOrUpdateParameter(new QueryParameter("filter", filter));
        return Client.PaginateCursor<ListAssetsResponse, AssetEntity>(request);
    }
    
    private Task<JObject> UploadToMount(MountResponse mount, byte[] fileBytes, string fileName, string contentType)
    {
        var request = new RestRequest(mount.Url, Method.Post)
        {
            AlwaysMultipartFormData = true,
            MultipartFormQuoteParameters = true 
        };

        foreach (var (key, value) in mount.FormData)
            request.AddParameter(key, value.ToString());

        request.AddFile("file", fileBytes, fileName, contentType);

        return ExternalClient.ExecuteWithErrorHandling<JObject>(request);
    }
}