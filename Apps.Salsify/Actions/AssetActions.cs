using Apps.Salsify.Api;
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
using RestSharp;

namespace Apps.Salsify.Actions;

[ActionList("Assets")]
public class AssetActions(InvocationContext context, IFileManagementClient fileManagementClient) : SalsifyInvocable(context)
{
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

    [Action("Get asset", Description = "Get details for a specific asset")]
    public async Task<AssetResponse> GetAsset([ActionParameter] AssetIdentifier assetIdentifier)
    {
        var request = new SalsifyRequest($"digital_assets/{assetIdentifier.AssetId}");
        var response = await Client.ExecuteWithErrorHandling<AssetEntity>(request);

        return new(response);
    }

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
}