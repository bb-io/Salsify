using Newtonsoft.Json;

namespace Apps.Salsify.Models.Utility.Current;

public class RolePropertyResponse
{
    [JsonProperty("id")]
    public string SystemId { get; set; } = string.Empty;
    
    [JsonProperty("external_id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("role")]
    public string Role { get; set; } = string.Empty;
}