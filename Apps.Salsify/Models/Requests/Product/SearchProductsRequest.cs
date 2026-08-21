using Apps.Salsify.Handlers;
using Apps.Salsify.Helpers.Validation.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Requests.Product;

public class SearchProductsRequest : IUpdatedDateRangeFilter
{
    [Display("Updated after")]
    public DateTime? UpdatedAfter { get; set; }

    [Display("Updated before")]
    public DateTime? UpdatedBefore { get; set; }

    [Display("Name contains")]
    public string? NameContains { get; set; }
    
    [Display("Product list ID"), DataSource(typeof(ProductListIdDataHandler))]
    public string? ListId { get; set; }
}