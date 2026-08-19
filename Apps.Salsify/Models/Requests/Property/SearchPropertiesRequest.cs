using Apps.Salsify.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Salsify.Models.Requests.Property;

public class SearchPropertiesRequest
{
    [Display("Property name contains")]
    public string? NameContains { get; set; }

    [Display("Search only localizable properties")]
    public bool? OnlyLocalizable { get; set; }

    [Display("Property type", Description = "String by default"), StaticDataSource(typeof(PropertyTypeDataHandler))]
    public string? Type { get; set; }
}