using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Constants;
using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers;
using Apps.Salsify.Models.Entities.Export;
using Apps.Salsify.Models.Entities.Product;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Requests.Product;
using Apps.Salsify.Models.Responses.File;
using Apps.Salsify.Models.Responses.Product;
using Apps.Salsify.Models.Responses.Product.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Salsify.Actions;

[ActionList("Products")]
public class ProductActions(InvocationContext context, IFileManagementClient fileManagementClient) : SalsifyInvocable(context)
{
    // https://developers.salsify.com/reference/bulk-read-products
    [Action("Search products", Description = "Search products")]
    public async Task<SearchProductsResponse> SearchProducts([ActionParameter] SearchProductsRequest searchInput)
    {
        searchInput.Validate();
        
        string nameProperty = await PropertyHelper.GetRolePropertyName(Client, RolePropertyNames.ProductName);

        var queryList = new List<string>();
        
        if (searchInput.UpdatedAfter.HasValue)
            queryList.Add($"'salsify:updated_at':gte('{searchInput.UpdatedAfter.Value.ToSalsifyStringDate()}')");
        
        if (searchInput.UpdatedBefore.HasValue)
            queryList.Add($"'salsify:updated_at':lte('{searchInput.UpdatedBefore.Value.ToSalsifyStringDate()}')");
        
        if (!string.IsNullOrEmpty(searchInput.NameContains))
            queryList.Add($"'{nameProperty}':contains('{searchInput.NameContains}')");
        
        if (!string.IsNullOrEmpty(searchInput.Query))
            queryList.Add(searchInput.Query.TrimStart('='));

        string? query = null;
        if (queryList.Count != 0)
            query = "=" + string.Join(',', queryList);
        
        var productsRequest = new SalsifyRequest("products").AddQueryParameterIfNotEmpty("filter", query);
        var productsResponse = await Client.PaginateCursor<ListProductsResponse, ProductEntity>(productsRequest);

        var result = productsResponse.Select(x => new ProductResponse(x, nameProperty)).ToArray();
        return new(result);
    }

    // https://developers.salsify.com/reference/read-product-record
    [Action("Get product", Description = "Get details for a specific product")]
    public async Task<ProductResponse> GetProduct([ActionParameter] ProductIdentifier productIdentifier)
    {
        string nameProperty = await PropertyHelper.GetRolePropertyName(Client, RolePropertyNames.ProductName);
        var request = new SalsifyRequest($"products/{productIdentifier.ProductId}");
        var response = await Client.ExecuteWithErrorHandling<ProductEntity>(request);
        return new(response, nameProperty);
    }

    // https://developers.salsify.com/reference/start-export-run
    [Action("Download product", Description = "Download product content")]
    public async Task<FileResponse> DownloadProduct(
        [ActionParameter] ProductIdentifier productIdentifier,
        [ActionParameter] DownloadProductRequest downloadInput)
    {
        // The filter param in the request below does not work with 'salsify:id' - it returns 'Invalid search query: Unknown error'
        // And the value that comes from productIdentifier uses product's 'salsify:id', not system ID
        // So we have to resolve it manually
        string productIdPropertyName = await PropertyHelper.GetRolePropertyName(Client, RolePropertyNames.ProductId);
        
        var startExportRequest = new SalsifyRequest("export_runs", Method.Post, ApiVersion.Unversioned)
            .WithJsonBody(new
            {
                configuration = new
                {
                    format = "json",
                    include_all_content_locales = true,
                    include_all_columns = true,
                    entity_type = "product",
                    filter = $"='{productIdPropertyName}':'{productIdentifier.ProductId}'"
                }
            });
        var startExportResponse = await Client.ExecuteWithErrorHandling<ExportResultEntity>(startExportRequest);
        
        long exportRunId = startExportResponse.Id;
        string downloadUrl = await PollForDownloadUrl(exportRunId);

        var s3Client = new RestClient();
        var downloadS3Request = new RestRequest(downloadUrl);
        var networkStream = await s3Client.DownloadStreamAsync(downloadS3Request) ??
                            throw new PluginApplicationException("Failed to download file from S3.");
        
        var seekableStream = new MemoryStream();
        await networkStream.CopyToAsync(seekableStream);
        seekableStream.Position = 0;

        var file = await fileManagementClient.UploadAsync(
            seekableStream, 
            "application/octet-stream", // TODO: change
            $"{productIdentifier.ProductId}.json");
        return new(file);
    }

    private async Task<string> PollForDownloadUrl(long exportRunId)
    {
        var getExportRequest = new SalsifyRequest($"export_runs/{exportRunId}", Method.Get, ApiVersion.Unversioned);

        int pollingTimeoutMinutes = 15;
        var pollingInterval = TimeSpan.FromSeconds(10);
        int maxAttempts = (int)(TimeSpan.FromMinutes(pollingTimeoutMinutes) / pollingInterval);

        string? downloadUrl = null;

        for (int attempts = 0; attempts < maxAttempts; attempts++)
        {
            var getExportResponse = await Client.ExecuteWithErrorHandling<ExportResultEntity>(getExportRequest);
            if (!getExportResponse.ExportEnded)
            {
                await Task.Delay(pollingInterval);
                continue;
            }
            
            if (getExportResponse.ExportEndedSuccessfully)
                downloadUrl = getExportResponse.Url;
            else
                throw new PluginApplicationException($"Product export failed. Fail reason: {getExportResponse.FailureReason ?? "unknown"}");

            break;
        }
        
        return downloadUrl ?? throw new PluginApplicationException($"Export run {exportRunId} did not finish within {pollingTimeoutMinutes} minutes");
    }
}