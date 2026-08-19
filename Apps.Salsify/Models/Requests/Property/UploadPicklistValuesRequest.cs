using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Salsify.Models.Requests.Property;

public class UploadPicklistValuesRequest
{
    [Display("Content")]
    public FileReference Content { get; set; } = null!;
}