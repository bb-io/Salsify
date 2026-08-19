using Apps.Salsify.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Identifiers;

public class PicklistIdentifier
{
    [Display("Picklist ID"), DataSource(typeof(PicklistPropertyDataHandler))]
    public string PicklistId { get; set; } = string.Empty;
}