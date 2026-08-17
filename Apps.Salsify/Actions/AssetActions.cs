using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers.Validation;
using Apps.Salsify.Models.Entities.Asset;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Requests.Asset;
using Apps.Salsify.Models.Responses.Asset;
using Apps.Salsify.Models.Responses.Asset.Api;
using Apps.Salsify.Models.Responses.File;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Salsify.Actions;

[ActionList("Assets")]
public class AssetActions(InvocationContext context, IFileManagementClient fileManagementClient) : SalsifyInvocable(context)
{
    // https://developers.salsify.com/reference/bulk-read-digital-assets
    [Action("Search assets", Description = "Search assets")]
    public async Task<SearchAssetsResponse> SearchAssets([ActionParameter] SearchAssetsRequest searchInput)
    {
        searchInput.ValidateDates();
        
        var queryList = new List<string>();
        
        if (searchInput.UpdatedAfter.HasValue)
            queryList.Add($"'salsify:updated_at':gte('{searchInput.UpdatedAfter.Value.ToSalsifyStringDate()}')");
        
        if (searchInput.UpdatedBefore.HasValue)
            queryList.Add($"'salsify:updated_at':lte('{searchInput.UpdatedBefore.Value.ToSalsifyStringDate()}')");
        
        if (!string.IsNullOrEmpty(searchInput.NameContains))
            queryList.Add($"'salsify:name':contains('{searchInput.NameContains}')");
        
        if (!string.IsNullOrEmpty(searchInput.Query))
            queryList.Add(searchInput.Query.TrimStart('='));
        
        string? query = null;
        if (queryList.Count != 0)
            query = "=" + string.Join(',', queryList);
        
        var request = new SalsifyRequest("digital_assets").AddQueryParameterIfNotEmpty("filter", query);
        var response = await Client.PaginateCursor<ListAssetsResponse, AssetEntity>(request);

        var result = response.Select(x => new AssetResponse(x)).ToArray();
        return new(result);
    }

    // https://developers.salsify.com/reference/get-digital-asset
    [Action("Get asset", Description = "Get details for a specific asset")]
    public async Task<AssetResponse> GetAsset([ActionParameter] AssetIdentifier assetIdentifier)
    {
        var asset = await FetchAsset(assetIdentifier.AssetId);
        return new(asset);
    }

    // https://developers.salsify.com/reference/digital-asset-object
    [Action("Download asset", Description = "Download a specific asset")]
    public async Task<FileResponse> DownloadAsset([ActionParameter] AssetIdentifier assetIdentifier)
    {
        string assetId = assetIdentifier.AssetId;
        
        var request = new SalsifyRequest($"digital_assets/{assetId}");
        var asset = await Client.ExecuteWithErrorHandling<AssetEntity>(request);

        if (string.IsNullOrWhiteSpace(asset.Url))
            throw new PluginMisconfigurationException($"Asset '{assetId}' does not have a download URL. Asset status - {asset.Status}");

        var downloadClient = new RestClient();
        var downloadRequest = new RestRequest(asset.Url);
        var downloadResponse = await downloadClient.ExecuteAsync(downloadRequest);
        if (!downloadResponse.IsSuccessful || downloadResponse.RawBytes is null or { Length: 0 })
            throw new PluginApplicationException($"Failed to download asset '{assetId}' ({(int)downloadResponse.StatusCode})");
        
        await using var stream = new MemoryStream(downloadResponse.RawBytes);
        var file = await fileManagementClient.UploadAsync(stream, downloadResponse.ContentType ?? "application/octet-stream", asset.Filename!);
        
        return new(file);
    }

    // Follow the UI asset upload flow to get all endpoints needed
    [Action("Upload asset", Description = "Create a new asset from a file")]
    public async Task<AssetResponse> UploadAsset([ActionParameter] UploadAssetRequest uploadInput)
    {
        await using var fileStream = await fileManagementClient.DownloadAsync(uploadInput.Content);
        var fileBytes = await fileStream.GetByteData();

        var mountBody = new Dictionary<string, object>
        {
            ["list_name"] = uploadInput.ListName,
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

        string fileName = uploadInput.Content.Name;
        string contentType = uploadInput.Content.ContentType;
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
        
        if (string.IsNullOrEmpty(uploadInput.Name))
            return new(asset);

        var updateNameBody = new Dictionary<string, string> { { "salsify:name", uploadInput.Name } };
        try
        {
            var updateNameRequest = new SalsifyRequest($"digital_assets/{asset.Id}", Method.Put).WithJsonBody(updateNameBody);
            await Client.ExecuteWithErrorHandling(updateNameRequest);
        }
        catch (PluginApplicationException exception)
        {
            InvocationContext.Logger?.LogError(
                $"Asset '{asset.Id}' was created but renaming it to '{uploadInput.Name}' failed: {exception.Message}. " +
                $"Current name is '{asset.Name}'.", []);
        }

        return new(asset);
    }

    // Follow the UI asset replacement flow to get all endpoints needed
    // For this, go to Assets -> any asset -> Actions -> Replace
    [Action("Replace asset", Description = "Replace an existing asset with a file")]
    public async Task ReplaceAsset(
        [ActionParameter] AssetIdentifier assetIdentifier,
        [ActionParameter] ReplaceAssetRequest replaceInput)
    {
        var asset = await FetchAsset(assetIdentifier.AssetId);
        
        await using var fileStream = await fileManagementClient.DownloadAsync(replaceInput.Content);
        var fileBytes = await fileStream.GetByteData();
        
        var mountRequest = new SalsifyRequest("digital_assets/mounts", Method.Post, ApiVersion.Unversioned);
        var mount = await Client.ExecuteWithErrorHandling<MountResponse>(mountRequest);

        string fileName = replaceInput.Content.Name;
        string contentType = replaceInput.Content.ContentType;
        var uploadResponse = await UploadToMount(mount, fileBytes, fileName, contentType);
        
        var replaceRequest = new SalsifyRequest($"digital_assets/{asset.SystemId}", Method.Put, ApiVersion.Unversioned)
            .WithJsonBody(new
            {
                data = new
                {
                    id = asset.SystemId,
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
        var request = new SalsifyRequest("digital_assets")
            .AddOrUpdateParameter(new QueryParameter("filter", filter));
        return Client.PaginateCursor<ListAssetsResponse, AssetEntity>(request);
    }

    private Task<AssetEntity> FetchAsset(string assetId)
    {
        var request = new SalsifyRequest($"digital_assets/{assetId}");
        return Client.ExecuteWithErrorHandling<AssetEntity>(request);
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