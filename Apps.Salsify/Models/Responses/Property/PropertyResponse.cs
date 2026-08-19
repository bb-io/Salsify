using Apps.Salsify.Models.Entities.Properties;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Models.Responses.Property;

public class PropertyResponse(PropertyEntity entity)
{
    [Display("Property ID")]
    public string Id { get; set; } = entity.Id;

    [Display("Property name")]
    public string Name { get; set; } = entity.Name;

    [Display("Property data type")]
    public string DataType { get; set; } = entity.DataType;

    [Display("Property type")]
    public string Type { get; set; } = entity.Type;

    [Display("Property role")]
    public string? Role { get; set; } = entity.Role;

    [Display("Property is localizable")]
    public bool Localizable { get; set; } = entity.Localizable;

    [Display("Property attribute group")]
    public string AttributeGroup { get; set; } = entity.AttributeGroup;

    [Display("Property position")]
    public int? Position { get; set; } = entity.Position;

    [Display("Property help text")]
    public string? HelpText { get; set; } = entity.HelpText;

    [Display("Property created at")]
    public DateTime CreatedAt { get; set; } = entity.CreatedAt;

    [Display("Property updated at")]
    public DateTime UpdatedAt { get; set; } = entity.UpdatedAt;
}