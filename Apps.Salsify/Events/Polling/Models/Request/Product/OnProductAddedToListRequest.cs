using Apps.Salsify.Handlers.List;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Events.Polling.Models.Request.Product;

public class OnProductAddedToListRequest
{   
    [Display("Product list ID"), DataSource(typeof(ProductListIdDataHandler))]
    public string ProductListId { get; set; } = string.Empty;
}