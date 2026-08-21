using Newtonsoft.Json;

namespace Apps.Salsify.Models.Entities.List;

public class ListEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("filter")]
    public string Filter { get; set; } = string.Empty;
}