using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Salsify.Models.Requests.LookupTable;

public class UpdateLookupTableRequest
{
    [Display("Content")]
    public FileReference Content { get; set; } = null!;
}