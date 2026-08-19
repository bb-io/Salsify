using Newtonsoft.Json;

namespace Apps.Salsify.Models.Responses.Asset.Api;

public class UploadListResponse
{
    [JsonProperty("id")] 
    public long Id { get; set; }
    
    [JsonProperty("system_id")]
    public string SystemId { get; set; } = string.Empty;
    
    [JsonProperty("name")] 
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("filter")] 
    public string Filter { get; set; } = string.Empty;
}