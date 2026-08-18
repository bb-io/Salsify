using Apps.Salsify.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Events.Webhooks.Models.Request.Asset;

public class OnAssetUpdatedRequest
{
    [Display("Asset IDs"), DataSource(typeof(AssetIdentifier))]
    public List<string>? AssetIds { get; set; }
}