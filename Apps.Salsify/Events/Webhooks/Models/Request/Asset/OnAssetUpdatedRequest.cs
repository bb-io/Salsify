using Apps.Salsify.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Events.Webhooks.Models.Request.Asset;

public class OnAssetUpdatedRequest
{
    [Display("Asset IDs"), DataSource(typeof(AssetDataHandler))]
    public List<string>? AssetIds { get; set; }
}