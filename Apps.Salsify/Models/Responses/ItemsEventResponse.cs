using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Models.Responses;

public class ItemsEventResponse : IMultiDownloadableContentOutput<ItemResponse>
{
    [Display("Items")] public List<ItemResponse> Items { get; set; } = new();
    [Display("Total count", Description = "The number of articles in this event.")] public int TotalCount { get; set; }
}
