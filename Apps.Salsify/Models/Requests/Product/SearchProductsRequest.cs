using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Models.Requests.Product;

public class SearchProductsRequest
{
    [Display("Filter query")]
    public string? Query { get; set; }
}