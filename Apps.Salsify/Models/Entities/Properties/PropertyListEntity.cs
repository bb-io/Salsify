using Newtonsoft.Json;

namespace Apps.Salsify.Models.Entities.Properties;

public class PropertyListEntity
{
    [JsonProperty("id")]
    public string SystemId { get; set; } = string.Empty;

    [JsonProperty("external_id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("data_type")]
    public string DataType { get; set; } = string.Empty;

    [JsonProperty("property_group")]
    public string PropertyGroup { get; set; } = string.Empty;
    
    [JsonProperty("localizable")]
    public bool Localizable { get; set; }
}