
using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Events.Polling.Models.Request.Product;

public class OnProductInListRequest : OnProductAddedToListRequest
{
    [Display("Limit", Description = "Maximum number of products to return. If not specified, all products are returned.")]
    public int? Limit { get; set; }
}
