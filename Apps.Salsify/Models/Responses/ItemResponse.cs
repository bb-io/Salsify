using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Models.Responses;

public class ItemResponse : IDownloadContentInput
{
    [Display("Content ID")]
    public string ContentId { get; set; } = string.Empty;
}