using Apps.Salsify.Constants;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salsify.Handlers.List;

public class AssetListNameDataHandler(InvocationContext context) : BaseListDataHandler(context), IAsyncDataSourceItemHandler
{
    protected override string ListEntityType => "digital_asset";
    protected override string ListType => ListTypes.Simple;
    
    public Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        return GetListItems(context, x => x.Name);
    }
}