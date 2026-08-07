using Newtonsoft.Json;

namespace Apps.Salsify.Models.Entities.Properties;

public class PropertyEntity
{
    [JsonProperty("salsify:id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("salsify:name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("salsify:data_type")]
    public string DataType { get; set; } = string.Empty;

    [JsonProperty("salsify:type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("salsify:role")]
    public string? Role { get; set; }

    [JsonProperty("salsify:localizable")]
    public bool Localizable { get; set; }

    [JsonProperty("salsify:attribute_group")]
    public string AttributeGroup { get; set; } = string.Empty;

    [JsonProperty("salsify:position")]
    public int? Position { get; set; }

    [JsonProperty("salsify:help_text")]
    public string? HelpText { get; set; }

    [JsonProperty("salsify:created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("salsify:updated_at")]
    public DateTime UpdatedAt { get; set; }
}