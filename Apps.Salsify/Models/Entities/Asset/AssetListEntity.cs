using Newtonsoft.Json;

namespace Apps.Salsify.Models.Entities.Asset;

public class AssetListEntity
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}