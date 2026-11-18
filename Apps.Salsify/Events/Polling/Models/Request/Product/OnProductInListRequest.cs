using Apps.Salsify.Handlers.List;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Events.Polling.Models.Request.Product;

public class OnProductInListRequest
{
    [Display("Product list ID"), DataSource(typeof(AnyProductListIdDataHandler))]
    public string ProductListId { get; set; } = string.Empty;
    
    [Display("Limit", Description = "Maximum number of products to return. If not specified, all products are returned.")]
    public int? Limit { get; set; }
}
