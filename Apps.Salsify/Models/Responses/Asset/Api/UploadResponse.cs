using Newtonsoft.Json;

namespace Apps.Salsify.Models.Responses.Asset.Api;

public class UploadResponse
{
    [JsonProperty("id")] 
    public long Id { get; set; }
    
    [JsonProperty("list")] 
    public UploadListResponse List { get; set; } = new();
}