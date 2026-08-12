using Apps.Salsify.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Requests.Product;

public class UpdatePropertyValueRequest
{
    [Display("Property value")] 
    public string PropertyValue { get; set; } = string.Empty;

    [Display("Locale"), DataSource(typeof(LocaleDataHandler))]
    public string? Locale { get; set; }
}