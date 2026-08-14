using Newtonsoft.Json;

namespace Apps.Salsify.Models.Entities.Properties;

public class PropertyGraphQlEntity
{
    [JsonProperty("externalId")] 
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}