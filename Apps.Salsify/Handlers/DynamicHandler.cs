using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salsify.Handlers;
public class DynamicHandler(InvocationContext invocationContext) : SalsifyInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
