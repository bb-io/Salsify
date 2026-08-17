using System.Net.Mime;
using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Constants.GraphQl;
using Apps.Salsify.Converters.Picklist;
using Apps.Salsify.Converters.Picklist.Models;
using Apps.Salsify.Extensions;
using Apps.Salsify.Models.Entities.Properties;
using Apps.Salsify.Models.Entities.Properties.Enumerated;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Identifiers.Optional;
using Apps.Salsify.Models.Requests.Property;
using Apps.Salsify.Models.Responses.File;
using Apps.Salsify.Models.Responses.Property;
using Apps.Salsify.Models.Responses.Property.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.System;
using Blackbird.Filters.Coders;
using Blackbird.Filters.Extensions;
using RestSharp;

namespace Apps.Salsify.Actions;

[ActionList("Properties")]
public class PropertyActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : SalsifyInvocable(invocationContext)
{
    [Action("Search properties", Description = "Search properties available in the organization")]
    public async Task<SearchPropertiesResponse> SearchProperties([ActionParameter] SearchPropertiesRequest input)
    {
        // This endpoint is not in the docs. It's accessible from DevTools
        // To access it, go to the UI homepage (product list) -> Customize View
        var request = new SalsifyRequest("properties", apiVersion: ApiVersion.Unversioned)
            .AddQueryParameter("use_new_serialization_format", "true")
            .AddQueryParameter("serialize_system_ids", "true")
            .AddQueryParameter("query_context", "name")
            .AddQueryParameterIfNotEmpty("query", input.NameContains);
        
        var response = await Client.PaginateOffset<ListPropertiesResponse, PropertyListEntity>(request);

        var filtered = response.AsEnumerable();
        if (input.OnlyLocalizable is true)
            filtered = filtered.Where(x => x.Localizable);

        if (!string.IsNullOrWhiteSpace(input.Type))
            filtered = filtered.Where(x => x.DataType.Equals(input.Type, StringComparison.OrdinalIgnoreCase));
        
        var result = filtered.Select(x => new PropertyListResponse(x)).ToArray();
        return new(result);
    }

    // https://developers.salsify.com/reference/read-property
    [Action("Get property", Description = "Get details for a specific property")]
    public async Task<PropertyResponse> GetProperty([ActionParameter] PropertyIdentifier propertyIdentifier)
    {
        var request = new SalsifyRequest($"properties/{propertyIdentifier.PropertyId}");
        var response = await Client.ExecuteWithErrorHandling<PropertyEntity>(request);
        return new(response);
    }

    // https://developers.salsify.com/reference/create-new-property
    [Action("Create property", Description = "Create a new property")]
    public async Task<PropertyResponse> CreateProperty([ActionParameter] CreatePropertyRequest createInput)
    {
        var body = new Dictionary<string, string?>
        {
            { "salsify:id", createInput.PropertyId },
            { "salsify:name", createInput.Name },
            { "salsify:data_type", createInput.Type },
        }.AllIsNotNull<string, string?>();

        var request = new SalsifyRequest("properties", Method.Post).WithJsonBody(body);
        var response = await Client.ExecuteWithErrorHandling<PropertyEntity>(request);
        return new(response);
    }
    
    // https://developers.salsify.com/reference/update-property
    [Action("Update property", Description = "Update an existing property")]
    public async Task UpdateProperty(
        [ActionParameter] PropertyIdentifier propertyIdentifier,
        [ActionParameter] UpdatePropertyRequest updateInput)
    {
        var body = new Dictionary<string, string>
        {
            { "salsify:name", updateInput.Name },
        };
        
        var request = new SalsifyRequest($"properties/{propertyIdentifier.PropertyId}", Method.Put).WithJsonBody(body);
        await Client.ExecuteWithErrorHandling(request);
    }

    [Action("Download picklist property values", Description = "Download picklist property values as HTML file")]
    public async Task<FileResponse> DownloadPicklistValues(
        [ActionParameter] PicklistIdentifier picklistIdentifier,
        [ActionParameter] LocaleOptionalIdentifier identifier)
    {
        var current = await Client.GetCurrentOrgInfo();
        string locale = current.ResolveLocale(identifier.Locale); 
        
        var picklistValuesResponse = await GetPicklistValues(picklistIdentifier.PicklistId, [locale]);
        var picklistValues = picklistValuesResponse
            .Select(x =>
            {
                string? localized = x.Names
                    .FirstOrDefault(n => string.Equals(n.Locale.Code, locale, StringComparison.OrdinalIgnoreCase))?
                    .Value;

                return new PicklistValue(x.Id, string.IsNullOrWhiteSpace(localized) ? x.Name : localized);
            })
            .ToList();

        var htmlDoc = PicklistHtmlConverter.GenerateHtml(picklistValues);
        string fileName = $"{picklistIdentifier.PicklistId}_{locale}.html";
        var coded = new HtmlCoder().Deserialize(htmlDoc.DocumentNode.OuterHtml, fileName);
        coded.Language = locale;
        coded.SystemReference.ContentId = picklistIdentifier.PicklistId;
        
        var outputFile = await fileManagementClient.UploadAsync(coded.ToStream(), MediaTypeNames.Text.Html, fileName);
        return new(outputFile);
    }

    [Action("Upload picklist property values", Description = "Upload picklist property values from a file")]
    public async Task UploadPicklistValues(
        [ActionParameter] UploadPicklistValuesRequest uploadInput,
        [ActionParameter] LocaleIdentifier localeIdentifier,
        [ActionParameter] PicklistOptionalIdentifier picklistIdentifier)
    {
        await using var fileStream = await fileManagementClient.DownloadAsync(uploadInput.Content);
        var htmlStream = await fileStream.ToHtmlTransformationStream(uploadInput.Content.Name);
        string html = htmlStream.ReadString();
        
        var coded = new HtmlCoder().Deserialize(html, uploadInput.Content.Name);
        string picklistId = 
            picklistIdentifier.PicklistId ?? 
            coded.SystemReference.ContentId ?? 
            throw new PluginMisconfigurationException("Picklist ID was not found in the file. Please provide it in the input");

        var current = await Client.GetCurrentOrgInfo();
        string locale = current.ValidateLocale(localeIdentifier.Locale);
        
        var translations = PicklistJsonConverter.ParseValues(html);
        if (translations.Count == 0)
            throw new PluginMisconfigurationException("The file contains no picklist values");
        
        var existingValues = await GetPicklistValues(picklistId, [locale]);
        var valuesByExternalId = existingValues.ToDictionary(x => x.Id, StringComparer.Ordinal);

        var missing = new List<string>();
        foreach (var (valueId, translation) in translations)
        {
            if (!valuesByExternalId.TryGetValue(valueId, out var existingValue))
            {
                missing.Add(valueId);
                continue;
            }

            if (string.Equals(existingValue.GetName(locale), translation, StringComparison.Ordinal))
                continue;

            // Updates are sequential so that we don't hit 422
            // Can be migrated to a batch GraphQL request later
            await UpdatePicklistValue(existingValue, locale, translation);
        }

        if (missing.Count > 0)
        {
            string warning = $"Skipped {missing.Count} value(s) that no longer exist in '{picklistId}': {string.Join(", ", missing)}";
            InvocationContext.Logger?.LogWarning(warning, []);
        }
    }
    
    private Task UpdatePicklistValue(EnumeratedValueEntity value, string locale, string translation)
    {
        // To access this endpoint, go to Properties -> Any picklist property -> Values,
        // then select any value -> Edit -> Localized Names
        var request = new GraphQlRequest("UpdateEnumeratedValue", GraphQlMutations.UpdateEnumeratedValue, new
        {
            input = new
            {
                id = value.SystemId,
                organizationId = OrgId,
                name = value.Name,
                localizedNames = new[]
                {
                    new
                    {
                        localeCode = locale, 
                        name = translation
                    }
                }
            },
            contentLocaleIds = new[] { locale }
        });

        return Client.ExecuteGraphQl(request);
    }

    public Task<List<EnumeratedValueEntity>> GetPicklistValues(string picklistId, List<string> locales)
    {
        // To access this endpoint, go to Properties -> Any picklist property -> Values
        var request = new GraphQlRequest("EnumeratedValues", GraphQlQueries.EnumeratedValues, new
        {
            propertyId = picklistId,
            flatten = true,
            contentLocalesCodes = locales
        });
        
        return Client.PaginateGraphQl<ListPropertyValuesResponse, EnumeratedValueEntity>(request);
    }
}