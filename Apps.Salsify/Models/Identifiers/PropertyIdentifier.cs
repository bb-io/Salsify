using Apps.Salsify.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Identifiers;

public class PropertyIdentifier
{
    [Display("Property ID"), DataSource(typeof(PropertyDataHandler))]
    public string PropertyId { get; set; } = string.Empty;
}