using Apps.Salsify.Handlers.Static;
using Apps.Salsify.Helpers.Validation.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Salsify.Models.Requests.Asset;

public class SearchAssetsRequest : IUpdatedDateRangeFilter
{
    [Display("Filter query", Description = "Without the '=' symbol at the beginning")]
    public string? Query { get; set; }
    
    [Display("Updated after")]
    public DateTime? UpdatedAfter { get; set; }

    [Display("Updated before")]
    public DateTime? UpdatedBefore { get; set; }

    [Display("Name contains")]
    public string? NameContains { get; set; }
}