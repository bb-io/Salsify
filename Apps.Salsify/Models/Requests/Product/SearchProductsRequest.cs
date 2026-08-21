using Apps.Salsify.Handlers;
using Apps.Salsify.Handlers.List;
using Apps.Salsify.Helpers.Validation;
using Apps.Salsify.Helpers.Validation.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Salsify.Models.Requests.Product;

public class SearchProductsRequest : IUpdatedDateRangeFilter
{
    [Display("Custom query", Description = "Without the '=' symbol at the beginning")]
    public string? CustomQuery { get; set; }
    
    [Display("Updated after")]
    public DateTime? UpdatedAfter { get; set; }

    [Display("Updated before")]
    public DateTime? UpdatedBefore { get; set; }

    [Display("Name contains")]
    public string? NameContains { get; set; }
    
    [Display("Product list ID"), DataSource(typeof(ProductListIdDataHandler))]
    public string? ListId { get; set; }

    [Display("Property names"), DataSource(typeof(PropertyDataHandler))]
    public List<string>? PropertyNames { get; set; }

    [Display("Property values", Description = "Corresponds to the 'Property names' input")]
    public List<string>? PropertyValues { get; set; }

    public void Validate()
    {
        this.ValidateDates();

        if (PropertyNames is not null && PropertyValues is not null && PropertyNames.Count != PropertyValues.Count)
            throw new PluginMisconfigurationException("Property inputs should have the same lenght");
    }
}