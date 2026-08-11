using Apps.Salsify.Api;
using Apps.Salsify.Api.Utility;
using Apps.Salsify.Models.Utility.Current;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Salsify.Handlers;

public class LocaleDataHandler(InvocationContext context) : SalsifyInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new SalsifyRequest("current", Method.Get, ApiVersion.Unversioned);
        var response = await Client.ExecuteWithErrorHandling<CurrentResponse>(request);

        var locales = response.Locales;
        if (!string.IsNullOrWhiteSpace(context.SearchString))
            locales = locales.Where(x => x.ToString().Contains(context.SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
        
        return locales.Select(x => new DataSourceItem(x.Id, x.ToString())).ToList();
    }
}