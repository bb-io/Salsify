using System.Net.Mime;
using Apps.Salsify.Api;
using Apps.Salsify.Constants;
using Apps.Salsify.Converters.Product;
using Apps.Salsify.Converters.Product.Models;
using Apps.Salsify.Extensions;
using Apps.Salsify.Helpers;
using Apps.Salsify.Helpers.Query;
using Apps.Salsify.Models.Entities.Product;
using Apps.Salsify.Models.Entities.Properties;
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
using Blackbird.Filters.Shared;
using RestSharp;

namespace Apps.Salsify.Actions;

[ActionList("Products")]
public class ProductActions(InvocationContext context, IFileManagementClient fileManagementClient) : SalsifyInvocable(context)
{
    // https://developers.salsify.com/reference/bulk-read-products
    [Action("Search products", Description = "Search for products using specific criteria. Fill in at least one advanced input field")]
    public async Task<SearchProductsResponse> SearchProducts([ActionParameter] SearchProductsRequest searchInput)
    {
        searchInput.Validate();

        var current = await Client.GetCurrentOrgInfo();
        string? nameProperty = current.GetRolePropertyId(RolePropertyNames.ProductName);
        
        var filterProperties = searchInput.PropertyNames?.ToArray() ?? [];
        var filterValues = searchInput.PropertyValues?.ToArray() ?? [];

        Filter?[] queryList =
        [
            Filter.GreaterOrEqual("salsify:updated_at", searchInput.UpdatedAfter),
            Filter.LessOrEqual("salsify:updated_at", searchInput.UpdatedBefore),
            Filter.Contains(nameProperty, searchInput.NameContains),
            Filter.InList(searchInput.ListId),
            Filter.Raw(searchInput.CustomQuery),
            ..filterProperties.Zip(filterValues, Filter.EqualTo)
        ];
        if (queryList.All(x => x is null))
            throw new PluginMisconfigurationException("Please fill at least one advanced input field first");
        
        string query = Filter.Build(queryList);
        
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
        [ActionParameter] DownloadProductRequest downloadInput,
        [ActionParameter] LocaleOptionalIdentifier identifier)
    {
        var current = await Client.GetCurrentOrgInfo();
        string locale = current.ResolveLocale(identifier.Locale);

        var getProductRequest = new SalsifyRequest($"products/{productIdentifier.ProductId}");
        var product = await Client.ExecuteWithErrorHandling<ProductEntity>(getProductRequest);
        
        var propertyKeys = product.Values.Select(x => x.Key);
        var propertyDefinitions = await PropertyHelper.GetDefinitions(Client, propertyKeys);

        var htmlOptions = new ProductHtmlOptions
        {
            Locale = locale,
            DefaultLocale = current.DefaultLocaleId,
            ExcludeProperties = downloadInput.ExcludeProperties ?? [],
            IncludeProperties = downloadInput.IncludeProperties ?? [],
            IncludeNonLocalizable = downloadInput.OnlyLocalizableProperties is false
        };
        var doc = ProductHtmlConverter.GenerateHtml(product, propertyDefinitions, htmlOptions);
        
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
        [ActionParameter] LocaleIdentifier localeIdentifier,
        [ActionParameter] ProductOptionalIdentifier productIdentifier)
    {
        await using var fileStream = await fileManagementClient.DownloadAsync(uploadInput.Content);
        string html = await fileStream.ToHtmlString(uploadInput.Content.Name);

        var coded = new HtmlCoder().Deserialize(html, uploadInput.Content.Name);
        string productId = productIdentifier.ProductId ?? 
                           coded.SystemReference.ContentId ??
                           throw new PluginMisconfigurationException("Product ID was not found in the file. Please provide it in the input");
        
        var current = await Client.GetCurrentOrgInfo();
        current.ValidateLocale(localeIdentifier.Locale);

        var values = ProductJsonConverter.ParseValues(html);
        if (values.Count == 0)
            throw new PluginMisconfigurationException("The file contains no property values");

        var definitions = await PropertyHelper.GetDefinitions(Client, values.Keys);
        var updateBody = ProductJsonConverter.BuildUpdateBody(values, definitions, localeIdentifier.Locale);
        if (updateBody.Count == 0)
            throw new PluginMisconfigurationException($"Nothing to write for product '{productId}'");

        var request = new SalsifyRequest($"products/{productId}", Method.Put).WithJsonBody(updateBody);
        await Client.ExecuteWithErrorHandling(request);
    }

    // https://developers.salsify.com/reference/add-a-product
    [Action("Create product", Description = "Create a new product")]
    public async Task<ProductResponse> CreateProduct([ActionParameter] CreateProductRequest createInput)
    {
        var current = await Client.GetCurrentOrgInfo();
        string idProperty = current.GetRolePropertyId(RolePropertyNames.ProductId) ?? 
                            throw new PluginMisconfigurationException("Product ID is not configured in your organization");

        var body = new Dictionary<string, string?>
        {
            { idProperty, createInput.Id }
        };

        string? nameProperty = null;
        if (!string.IsNullOrWhiteSpace(createInput.Name))
        {
            nameProperty = current.GetRolePropertyId(RolePropertyNames.ProductName) ?? 
                           throw new PluginMisconfigurationException("Product name is not configured in your organization");
            body[nameProperty] = createInput.Name;
        }

        var request = new SalsifyRequest("products", Method.Post).AddJsonBody(body);
        var response = await Client.ExecuteWithErrorHandling<ProductEntity>(request);
        return new(response, nameProperty);
    }

    // https://developers.salsify.com/reference/delete-product
    [Action("Delete product", Description = "Delete an existing product, including all associated stored values")]
    public Task DeleteProduct([ActionParameter] ProductIdentifier productIdentifier)
    {
        var request = new SalsifyRequest($"products/{productIdentifier.ProductId}", Method.Delete);
        return Client.ExecuteWithErrorHandling(request);
    }

    // https://developers.salsify.com/reference/update-product
    [Action("Update product property value", Description = "Update property value of a specific product")]
    public async Task UpdatePropertyValue(
        [ActionParameter] ProductIdentifier productIdentifier,
        [ActionParameter] PropertyIdentifier propertyIdentifier,
        [ActionParameter] UpdatePropertyValueRequest updateInput,
        [ActionParameter] LocaleOptionalIdentifier identifier)
    {
        string propertyId = propertyIdentifier.PropertyId;
        string productId = productIdentifier.ProductId;
        string? locale = identifier.Locale;

        var propertyRequest = new SalsifyRequest($"properties/{propertyId}");
        var property = await Client.ExecuteWithErrorHandling<PropertyEntity>(propertyRequest);

        if (property.Type == PropertyTypeConstants.ComputedPropertyType)
            throw new PluginMisconfigurationException($"Property '{propertyId}' is computed and cannot be written to.");

        var current = await Client.GetCurrentOrgInfo();
        if (property.Localizable && string.IsNullOrWhiteSpace(locale))
            throw new PluginMisconfigurationException($"Property '{propertyId}' is localizable - specify which locale to update");

        if (!string.IsNullOrWhiteSpace(locale))
            current.ValidateLocale(locale);

        bool localized = property.Localizable && !string.IsNullOrWhiteSpace(locale);
        if (!localized && !string.IsNullOrWhiteSpace(locale))
        {
            string logMsg = $"Property '{propertyId}' is not localizable - the locale is ignored and the single value is replaced";
            InvocationContext.Logger?.LogInformation(logMsg, []);
        }

        var body = new Dictionary<string, object>
        {
            [propertyId] = localized
                ? new Dictionary<string, string> { [locale!] = updateInput.PropertyValue }
                : updateInput.PropertyValue
        };

        var updateRequest = new SalsifyRequest($"products/{productId}", Method.Put).WithJsonBody(body);
        await Client.ExecuteWithErrorHandling(updateRequest);
    }
}