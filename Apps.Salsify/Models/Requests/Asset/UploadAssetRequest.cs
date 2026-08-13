using Apps.Salsify.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Salsify.Models.Requests.Asset;

public class UploadAssetRequest
{
    [Display("Content")]
    public FileReference Content { get; set; } = null!;

    [Display("Custom asset name")]
    public string? Name { get; set; }

    [Display("Asset list name"), DataSource(typeof(AssetListNameDataHandler))]
    public string ListName { get; set; } = string.Empty;
}