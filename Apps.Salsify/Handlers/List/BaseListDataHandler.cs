using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Extensions;
using Apps.Salsify.Models.Entities.List;
using Apps.Salsify.Models.Responses.List.Api;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Salsify.Handlers.List;

public abstract class BaseListDataHandler(InvocationContext context) : SalsifyInvocable(context)
{
    protected abstract string ListEntityType { get; }

    protected async Task<IEnumerable<DataSourceItem>> GetListItems(DataSourceContext context, Func<ListEntity, string> idSelector)
    {
        var request = new SalsifyRequest("lists", Method.Get, ApiVersion.Unversioned)
            .AddQueryParameter("entity_type", ListEntityType)
            .AddQueryParameter("type", "simple")
            .AddQueryParameterIfNotEmpty("query", context.SearchString);

        var response = await Client.PaginateOffset<ListListsResponse, ListEntity>(request, paginateTimes: 2);
        return response.Select(x => new DataSourceItem(idSelector(x), x.Name)).ToList();
    }
}