using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Extensions;
using Apps.Salsify.Models.Entities.Properties;
using Apps.Salsify.Models.Identifiers;
using Apps.Salsify.Models.Requests.Property;
using Apps.Salsify.Models.Responses.Property;
using Apps.Salsify.Models.Responses.Property.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.System;
using RestSharp;

namespace Apps.Salsify.Actions;

[ActionList("Properties")]
public class PropertyActions(InvocationContext invocationContext) : SalsifyInvocable(invocationContext)
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
}