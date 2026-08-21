using Apps.Salsify.Helpers.Validation;
using Apps.Salsify.Helpers.Validation.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Salsify.Models.Requests.Product;

public class SearchProductsRequest : IUpdatedDateRangeFilter
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
        if (string.IsNullOrEmpty(Query) &&
            string.IsNullOrEmpty(NameContains) &&
            !UpdatedAfter.HasValue &&
            !UpdatedBefore.HasValue)
        {
            throw new PluginMisconfigurationException("Please fill at least one advanced input field first");
        }
        
        this.ValidateDates();
    }
}