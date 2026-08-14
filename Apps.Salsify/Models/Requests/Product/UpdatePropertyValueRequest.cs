using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Models.Requests.Product;

public class UpdatePropertyValueRequest
{
    [Display("Property value")] 
    public string PropertyValue { get; set; } = string.Empty;
}