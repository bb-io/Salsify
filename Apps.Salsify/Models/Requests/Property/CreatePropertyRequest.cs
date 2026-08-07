using Apps.Salsify.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Salsify.Models.Requests.Property;

public class CreatePropertyRequest
{
    [Display("Property ID", Description = "Unique case sensitive identifier for property. Cannot start with 'salsify', = or _")]
    public string PropertyId { get; set; } = string.Empty;

    [Display("Property type", Description = "String by default"), StaticDataSource(typeof(PropertyTypeDataHandler))]
    public string? Type { get; set; }

    [Display("Property name", Description = "If not specified, property name will be filled with property ID value (recommended)")]
    public string? Name { get; set; }
}