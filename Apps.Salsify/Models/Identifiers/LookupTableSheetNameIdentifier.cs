using Apps.Salsify.Handlers.LookupTable;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Identifiers;

public class LookupTableSheetNameIdentifier
{
    [Display("Sheet name"), DataSource(typeof(LookupTableSheetNameDataHandler))]
    public string SheetName { get; set; } = string.Empty;
}