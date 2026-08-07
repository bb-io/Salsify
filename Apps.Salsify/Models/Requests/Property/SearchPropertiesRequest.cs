using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Models.Requests.Property;

public class SearchPropertiesRequest
{
    [Display("Property name contains")]
    public string? NameContains { get; set; }
}