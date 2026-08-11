using System.Net.Mime;
using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Constants;
using Apps.Salsify.Converters.Product;
using Apps.Salsify.Converters.Product.Models;
using Apps.Salsify.Extensions;
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
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Filters.Coders;
using Blackbird.Filters.Shared;
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

        var current = await Client.GetCurrentOrgInfo();
        string nameProperty = current.GetRolePropertyId(RolePropertyNames.ProductName);

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
        var current = await Client.GetCurrentOrgInfo();
        string nameProperty = current.GetRolePropertyId(RolePropertyNames.ProductName);
        
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
        var current = await Client.GetCurrentOrgInfo();
        
        string locale = current.ResolveLocale(downloadInput.Locale);
        string nameProperty = current.GetRolePropertyId(RolePropertyNames.ProductName);
        string productIdProperty = current.GetRolePropertyId(RolePropertyNames.ProductId) ?? 
                                   throw new PluginMisconfigurationException("This organization has no product ID property configured");
        
        var startExportRequest = new SalsifyRequest("export_runs", Method.Post, ApiVersion.Unversioned)
            .WithJsonBody(new
            {
                configuration = new
                {
                    format = "json",
                    include_all_content_locales = true,
                    include_all_columns = true,
                    entity_type = "product",
                    filter = $"='{productIdProperty}':'{productIdentifier.ProductId}'"
                }
            });
        var startExportResponse = await Client.ExecuteWithErrorHandling<ExportResultEntity>(startExportRequest);
        
        long exportRunId = startExportResponse.Id;
        string downloadUrl = await PollForDownloadUrl(exportRunId);

        var s3Client = new RestClient();
        var downloadS3Request = new RestRequest(downloadUrl);
        var downloadS3Response = await s3Client.ExecuteAsync(downloadS3Request);

        if (!downloadS3Response.IsSuccessful || string.IsNullOrEmpty(downloadS3Response.Content))
            throw new PluginApplicationException($"Failed to download export file ({(int)downloadS3Response.StatusCode}).");

        string exportedJson = downloadS3Response.Content;
        var productContent = new ProductJsonContent(exportedJson);
        var product = productContent.Products.FirstOrDefault() ?? 
                      throw new PluginMisconfigurationException("The export contained no products");
        
        var doc = ProductHtmlConverter.GenerateHtml(
            productContent, 
            locale, 
            includeNonLocalizable: downloadInput.OnlyLocalizableProperties is false, 
            downloadInput.ExcludeProperties);
        
        string? productName = nameProperty is null ? null : product.GetValue(nameProperty);
        string fileName = $"{product.Id}_{locale}.html";
        var coded = new HtmlCoder().Deserialize(doc.DocumentNode.OuterHtml, fileName);

        coded.Language = locale;
        coded.Metadata["blackbird-salsify-version"] = product.Version.ToString();
        coded.SystemReference = new SystemReference
        {
            ContentId = product.Id,
            ContentName = productName ?? product.Id,
            SystemName = "Salsify",
            SystemRef = "https://app.salsify.com/",
            AdminUrl = $"https://app.salsify.com/app/orgs/{Creds.Get(CredsNames.OrgId).Value}/products/v2/{product.Id}"
        };

        var outputFile = await fileManagementClient.UploadAsync(coded.ToStream(), MediaTypeNames.Text.Html, fileName);
        return new(outputFile);
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