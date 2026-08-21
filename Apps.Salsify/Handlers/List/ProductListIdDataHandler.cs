using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salsify.Handlers.List;

public class ProductListIdDataHandler(InvocationContext context) : BaseListDataHandler(context), IAsyncDataSourceItemHandler
{
    protected override string ListEntityType => "product";
    
    public Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        return GetListItems(context, x => x.Id);
    }
}