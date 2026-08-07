using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Handlers.Static;

public class PropertyTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return [
            new DataSourceItem("boolean", "Boolean"),
            new DataSourceItem("date", "Date"),
            new DataSourceItem("digital_asset", "Digital Asset"),
            new DataSourceItem("html", "HTML"),
            new DataSourceItem("link", "Link"),
            new DataSourceItem("number", "Number"),
            new DataSourceItem("enumerated", "Picklist/Category"),
            new DataSourceItem("quantified_record", "Quantified Reference"),
            new DataSourceItem("record", "Reference"),
            new DataSourceItem("rich_text", "Rich Text"),
            new DataSourceItem("string", "String")
        ];
    }
}
