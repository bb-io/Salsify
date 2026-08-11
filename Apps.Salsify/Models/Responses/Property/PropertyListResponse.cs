using Apps.Salsify.Models.Entities.Properties;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Salsify.Models.Responses.Property;

public class PropertyListResponse(PropertyListEntity propertyListEntity)
{
    [Display("Property ID")]
    public string Id { get; set; } = propertyListEntity.Id;

    [Display("Property name")] 
    public string Name { get; set; } = propertyListEntity.Name;

    [Display("Property data type")]
    public string DataType { get; set; } = propertyListEntity.DataType;

    [Display("Property group")]
    public string PropertyGroup { get; set; } = propertyListEntity.PropertyGroup;
    
    [Display("Property is localizable")]
    public bool IsLocalizable { get; set; } = propertyListEntity.Localizable;
}