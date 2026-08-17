using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Salsify.Models.Requests.Asset;

public class ReplaceAssetRequest
{
    [Display("Content")]
    public FileReference Content { get; set; } = null!;
}