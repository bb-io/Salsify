using Apps.Salsify.Handlers.List;
using Apps.Salsify.Helpers.Validation.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Requests.Asset;

public class SearchAssetsRequest : IUpdatedDateRangeFilter
{
    [Display("Custom query", Description = "Without the '=' symbol at the beginning")]
    public string? CustomQuery { get; set; }
    
    [Display("Updated after")]
    public DateTime? UpdatedAfter { get; set; }

    [Display("Updated before")]
    public DateTime? UpdatedBefore { get; set; }

    [Display("Name contains")]
    public string? NameContains { get; set; }

    [Display("Asset list ID"), DataSource(typeof(AssetListIdDataHandler))]
    public string? ListId { get; set; }
}