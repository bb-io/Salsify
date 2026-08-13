using Newtonsoft.Json;

namespace Apps.Salsify.Models.Entities.Asset;

public class AssetEntity
{
    [JsonProperty("salsify:id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("salsify:url")]
    public string? Url { get; set; }

    [JsonProperty("salsify:name")]
    public string? Name { get; set; }

    [JsonProperty("salsify:created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("salsify:updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty("salsify:status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("salsify:asset_resource_type")]
    public string? ResourceType { get; set; }

    [JsonProperty("salsify:filename")]
    public string? Filename { get; set; }

    [JsonProperty("salsify:bytes")]
    public int Bytes { get; set; }

    [JsonProperty("salsify:format")]
    public string? Format { get; set; }

    public override string ToString()
    {
        string displayName = string.IsNullOrWhiteSpace(Name) ? Id : Name;
        return $"{displayName} (Status: {Status})";
    }
}