using Apps.Salsify.Api;
using Apps.Salsify.Constants.GraphQl;
using Apps.Salsify.Models.Entities.Properties;
using Apps.Salsify.Models.Responses.Property.Api;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salsify.Handlers;

public class PicklistPropertyDataHandler(InvocationContext invocationContext) 
    : SalsifyInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new GraphQlRequest("PropertyIndex", GraphQlQueries.PropertyIndex, new
        {
            query = context.SearchString ?? string.Empty,
            orderBy = new[]
            {
                new
                {
                    field = "EXTERNAL_ID",
                    direction = "ASC"
                }
            },
            dataType = new[] { "ENUMERATED" }
        });
        
        var properties = await GraphQlClient.Paginate<ListPropertiesGraphQlResponse, PropertyGraphQlEntity>(request, paginateTimes: 1);
        return properties.Select(x => new DataSourceItem(x.Id, x.Name));
    }
}
