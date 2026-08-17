using Apps.Salsify.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Identifiers;

public class LookupTableIdentifier
{
    [Display("Table asset ID"), DataSource(typeof(TableAssetDataHandler))]
    public string AssetId { get; set; } = string.Empty;
}