using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Events.Polling.Models.Request.Property;

public class OnPropertyCreatedOrUpdatedRequest
{
    [Display("Include only localizable properties", Description = "Default is false - all properties are included")]
    public bool? OnlyLocalizable { get; set; }
}