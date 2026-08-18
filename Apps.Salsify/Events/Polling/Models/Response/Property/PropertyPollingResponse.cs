using Apps.Salsify.Models.Entities.Properties;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Events.Polling.Models.Response.Property;

public class PropertyPollingResponse(PropertyPollingEntity entity)
{
    [Display("Property ID")] 
    public string Id { get; set; } = entity.Id;

    [Display("Property name")] 
    public string Name { get; set; } = entity.Name;

    [Display("Property is localizable")]
    public bool IsLocalizable { get; set; } = entity.IsLocalizable;

    [Display("Property created at")]
    public DateTime CreatedAt { get; set; } = entity.CreatedAt;

    [Display("Property updated at")]
    public DateTime UpdatedAt { get; set; } = entity.UpdatedAt;
}