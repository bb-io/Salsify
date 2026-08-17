using Apps.Salsify.Api;
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

namespace Apps.Salsify.Actions;

[ActionList("Assets")]
public class AssetActions(InvocationContext context, IFileManagementClient fileManagementClient) : SalsifyInvocable(context)
{
    private readonly AssetHelper _assetHelper = new(context);
    
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
        var asset = await _assetHelper.GetAsset(assetIdentifier.AssetId);
        return new(asset);
    }

    // https://developers.salsify.com/reference/digital-asset-object
    [Action("Download asset", Description = "Download a specific asset")]
    public async Task<FileResponse> DownloadAsset([ActionParameter] AssetIdentifier assetIdentifier)
    {
        var downloadedAsset = await _assetHelper.DownloadAsset(assetIdentifier.AssetId);
        
        await using var stream = new MemoryStream(downloadedAsset.Bytes);
        var file = await fileManagementClient.UploadAsync(stream, downloadedAsset.ContentType, downloadedAsset.Filename);
        
        return new(file);
    }

    // Follow the UI asset upload flow to get all endpoints needed
    [Action("Upload asset", Description = "Create a new asset from a file")]
    public async Task<AssetResponse> UploadAsset([ActionParameter] UploadAssetRequest uploadInput)
    {
        await using var fileStream = await fileManagementClient.DownloadAsync(uploadInput.Content);
        var fileBytes = await fileStream.GetByteData();

        string fileName = uploadInput.Content.Name;
        string contentType = uploadInput.Content.ContentType;
        
        var uploadedAsset = await _assetHelper.UploadAsset(fileBytes, fileName, contentType, uploadInput.ListName);
        string uploadedAssetId = uploadedAsset.Id;
        
        if (string.IsNullOrEmpty(uploadInput.Name))
            return new(uploadedAsset);

        try
        {
            await _assetHelper.RenameAsset(uploadedAssetId, uploadInput.Name);
            var updatedAsset = await _assetHelper.GetAsset(uploadedAssetId);
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
    [Action("Update asset", Description = "Replace an existing asset with a file")]
    public async Task UpdateAsset(
        [ActionParameter] AssetIdentifier assetIdentifier,
        [ActionParameter] UpdateAssetRequest updateInput)
    {
        await using var fileStream = await fileManagementClient.DownloadAsync(updateInput.Content);
        var fileBytes = await fileStream.GetByteData();

        await _assetHelper.ReplaceAsset(fileBytes, assetIdentifier.AssetId, updateInput.Content.Name, updateInput.Content.ContentType);
    }
}