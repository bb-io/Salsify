using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Extensions;
using Apps.Salsify.Models.Entities.Properties;
using Apps.Salsify.Models.Responses.Property.Api;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Salsify.Handlers;

public class PropertyDataHandler(InvocationContext invocationContext) : SalsifyInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new SalsifyRequest("properties", apiVersion: ApiVersion.Unversioned)
            .AddQueryParameter("use_new_serialization_format", "true")
            .AddQueryParameter("serialize_system_ids", "true")
            .AddQueryParameter("query_context", "name")
            .AddQueryParameterIfNotEmpty("query", context.SearchString);
        
        var properties = await Client.PaginateOffset<ListPropertiesResponse, PropertyListEntity>(request, paginateTimes: 2);
        return properties.Select(x => new DataSourceItem(x.Id, $"{x.Name} ({x.PropertyGroup})"));
    }
}
