using Newtonsoft.Json;

namespace Apps.Salsify.Models.Entities.Properties;

public class PropertyPollingEntity
{
    [JsonProperty("externalId")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("localizable")]
    public bool IsLocalizable { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}