using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Salsify.Models.Responses.Asset.Api;

public class MountResponse
{
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
    
    [JsonProperty("form_data")] 
    public Dictionary<string, JToken> FormData { get; set; } = new();
}