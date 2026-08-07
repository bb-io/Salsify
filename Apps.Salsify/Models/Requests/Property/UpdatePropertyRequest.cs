using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Models.Requests.Property;

public class UpdatePropertyRequest
{
    [Display("New property name")]
    public string Name { get; set; } = string.Empty;
}