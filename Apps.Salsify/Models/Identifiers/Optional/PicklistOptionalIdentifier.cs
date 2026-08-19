using Apps.Salsify.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Salsify.Models.Identifiers.Optional;

public class PicklistOptionalIdentifier
{
    [Display("Picklist ID"), DataSource(typeof(PicklistPropertyDataHandler))]
    public string? PicklistId { get; set; }
}