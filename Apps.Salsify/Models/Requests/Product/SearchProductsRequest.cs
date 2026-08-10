using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Salsify.Models.Requests.Product;

public class SearchProductsRequest
{
    [Display("Filter query", Description = "Without the '=' symbol at the beginning")]
    public string? Query { get; set; }

    [Display("Updated after")]
    public DateTime? UpdatedAfter { get; set; }

    [Display("Updated before")]
    public DateTime? UpdatedBefore { get; set; }

    [Display("Name contains")]
    public string? NameContains { get; set; }

    public void Validate()
    {
        if (UpdatedAfter.HasValue && UpdatedBefore.HasValue && UpdatedAfter.Value > UpdatedBefore.Value)
            throw new PluginMisconfigurationException("Invalid date range - date after can't be later that date before");
    }
}