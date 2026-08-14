using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Handlers.Static;

public class PropertyTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return [
            new DataSourceItem("string", "String"),
            new DataSourceItem("number", "Number"),
            new DataSourceItem("enumerated", "Picklist/Category"),
            new DataSourceItem("boolean", "Boolean"),
            new DataSourceItem("date", "Date"),
            new DataSourceItem("html", "HTML"),
            new DataSourceItem("rich_text", "Rich Text"),
            new DataSourceItem("link", "Link"),
            new DataSourceItem("digital_asset", "Digital Asset")
        ];
    }
}
