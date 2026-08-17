using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers.Asset;
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
        var helper = new AssetHelper(InvocationContext);
        var asset = await helper.GetAsset(assetIdentifier.AssetId);
        return new(asset);
    }

    // https://developers.salsify.com/reference/digital-asset-object
    [Action("Download asset", Description = "Download a specific asset")]
    public async Task<FileResponse> DownloadAsset([ActionParameter] AssetIdentifier assetIdentifier)
    {
        var assetHelper = new AssetHelper(InvocationContext);
        var downloadedAsset = await assetHelper.DownloadAsset(assetIdentifier.AssetId);
        
        await using var stream = new MemoryStream(downloadedAsset.Bytes);
        var file = await fileManagementClient.UploadAsync(stream, downloadedAsset.ContentType, downloadedAsset.Filename);
        
        return new(file);
    }

    // Follow the UI asset upload flow to get all endpoints needed
    [Action("Upload asset", Description = "Create a new asset from a file")]
    public async Task<AssetResponse> UploadAsset([ActionParameter] UploadAssetRequest uploadInput)
    {
        var helper = new AssetHelper(InvocationContext);
        
        await using var fileStream = await fileManagementClient.DownloadAsync(uploadInput.Content);
        var fileBytes = await fileStream.GetByteData();

        string fileName = uploadInput.Content.Name;
        string contentType = uploadInput.Content.ContentType;
        
        var uploadedAsset = await helper.UploadAsset(fileBytes, fileName, contentType, uploadInput.ListName);
        string uploadedAssetId = uploadedAsset.Id;
        
        if (string.IsNullOrEmpty(uploadInput.Name))
            return new(uploadedAsset);

        try
        {
            await helper.RenameAsset(uploadedAssetId, uploadInput.Name);
            var updatedAsset = await helper.GetAsset(uploadedAssetId);
            return new(updatedAsset);
        }
        catch (PluginApplicationException exception)
        {
            InvocationContext.Logger?.LogError(
                $"Asset '{uploadedAssetId}' was created but renaming it to '{uploadInput.Name}' failed: {exception.Message}. " +
                $"Current name is '{uploadedAsset.Name}'.", []);
            return new(uploadedAsset);
        }
    }

    // Follow the UI asset replacement flow to get all endpoints needed
    // For this, go to Assets -> any asset -> Actions -> Replace
    [Action("Replace asset", Description = "Replace an existing asset with a file")]
    public async Task ReplaceAsset(
        [ActionParameter] AssetIdentifier assetIdentifier,
        [ActionParameter] ReplaceAssetRequest replaceInput)
    {
        var helper = new AssetHelper(InvocationContext);
        var asset = await helper.GetAsset(assetIdentifier.AssetId);
        
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