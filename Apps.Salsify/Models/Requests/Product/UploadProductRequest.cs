using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Salsify.Models.Requests.Product;

public class UploadProductRequest
{
    [Display("File content")]
    public FileReference Content { get; set; } = null!;
}