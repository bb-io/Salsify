using System.Net.Mime;
using Apps.Salsify.Api;
using Apps.Salsify.Constants;
using Apps.Salsify.Converters.Product;
using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers;
using Apps.Salsify.Models.Entities.Product;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Identifiers.Optional;
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
using Blackbird.Filters.Extensions;
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
        string? nameProperty = current.GetRolePropertyId(RolePropertyNames.ProductName);

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
        string? nameProperty = current.GetRolePropertyId(RolePropertyNames.ProductName);
        
        var request = new SalsifyRequest($"products/{productIdentifier.ProductId}");
        var response = await Client.ExecuteWithErrorHandling<ProductEntity>(request);
        return new(response, nameProperty);
    }

    // https://developers.salsify.com/reference/read-product-record
    [Action("Download product", Description = "Download product content")]
    public async Task<FileResponse> DownloadProduct(
        [ActionParameter] ProductIdentifier productIdentifier,
        [ActionParameter] DownloadProductRequest downloadInput)
    {
        var current = await Client.GetCurrentOrgInfo();
        string locale = current.ResolveLocale(downloadInput.Locale);

        var getProductRequest = new SalsifyRequest($"products/{productIdentifier.ProductId}");
        var product = await Client.ExecuteWithErrorHandling<ProductEntity>(getProductRequest);
        
        var propertyKeys = product.Values.Select(x => x.Key);
        var propertyDefinitions = await PropertyHelper.GetDefinitions(Client, propertyKeys);
        
        var doc = ProductHtmlConverter.GenerateHtml(
            product, 
            propertyDefinitions,
            locale, 
            includeNonLocalizable: downloadInput.OnlyLocalizableProperties is false, 
            downloadInput.ExcludeProperties);
        
        string? nameProperty = current.GetRolePropertyId(RolePropertyNames.ProductName);
        string? productName = nameProperty is null ? null : product.GetValue(nameProperty);
        string fileName = $"{product.Id}_{locale}.html";
        
        var coded = new HtmlCoder().Deserialize(doc.DocumentNode.OuterHtml, fileName);
        coded.Language = locale;
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

    // https://developers.salsify.com/reference/update-product
    [Action("Upload product", Description = "Upload product content from a file")]
    public async Task UploadProduct(
        [ActionParameter] UploadProductRequest uploadInput,
        [ActionParameter] ProductOptionalIdentifier productIdentifier)
    {
        await using var fileStream = await fileManagementClient.DownloadAsync(uploadInput.Content);
        var htmlStream = await fileStream.ToHtml(uploadInput.Content.Name);
        string html = htmlStream.ReadString();

        var coded = new HtmlCoder().Deserialize(html, uploadInput.Content.Name);
        string productId = productIdentifier.ProductId ?? 
                           coded.SystemReference.ContentId ??
                           throw new PluginMisconfigurationException("Product ID was not found in the file. Please provide it in the input");
        
        var current = await Client.GetCurrentOrgInfo();
        if (!current.Locales.Select(x => x.Id).Contains(uploadInput.Locale))
        {
            throw new PluginMisconfigurationException(
                $"Locale '{uploadInput.Locale}' is not configured. " +
                $"Available: {string.Join(", ", current.Locales.Select(x => x.Id))}");
        }

        var values = ProductJsonConverter.ParseValues(html);
        if (values.Count == 0)
            throw new PluginMisconfigurationException("The file contains no property values");

        var definitions = await PropertyHelper.GetDefinitions(Client, values.Keys);
        var updateBody = ProductJsonConverter.BuildUpdateBody(values, definitions, uploadInput.Locale);
        if (updateBody.Count == 0)
            throw new PluginMisconfigurationException($"Nothing to write for product '{productId}'");

        var request = new SalsifyRequest($"products/{productId}", Method.Put).WithJsonBody(updateBody);
        await Client.ExecuteWithErrorHandling(request);
    }

    // https://developers.salsify.com/reference/add-a-product
    [Action("Create product", Description = "Create a new product")]
    public async Task CreateProduct([ActionParameter] CreateProductRequest createInput)
    {
        var current = await Client.GetCurrentOrgInfo();
        string idProperty = current.GetRolePropertyId(RolePropertyNames.ProductId) ?? 
                            throw new PluginMisconfigurationException("Product ID is not configured in your organization");

        var body = new Dictionary<string, string?>
        {
            { idProperty, createInput.Id }
        };

        if (!string.IsNullOrWhiteSpace(createInput.Name))
        {
            string nameProperty = current.GetRolePropertyId(RolePropertyNames.ProductName) ?? 
                                  throw new PluginMisconfigurationException("Product name is not configured in your organization");
            body[nameProperty] = createInput.Name;
        }

        var request = new SalsifyRequest("products", Method.Post).AddJsonBody(body);
        await Client.ExecuteWithErrorHandling(request);
    }

    // https://developers.salsify.com/reference/delete-product
    [Action("Delete product", Description = "Delete an existing product, including all associated stored values")]
    public Task DeleteProduct([ActionParameter] ProductIdentifier productIdentifier)
    {
        var request = new SalsifyRequest($"products/{productIdentifier.ProductId}", Method.Delete);
        return Client.ExecuteWithErrorHandling(request);
    }
}