using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Salsify.Models.Requests.Asset;

public class UpdateAssetRequest
{
    [Display("Content")]
    public FileReference Content { get; set; } = null!;
}