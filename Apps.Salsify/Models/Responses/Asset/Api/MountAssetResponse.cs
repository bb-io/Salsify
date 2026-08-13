using Newtonsoft.Json;

namespace Apps.Salsify.Models.Responses.Asset.Api;

public class MountAssetResponse
{
    [JsonProperty("mount")] 
    public MountResponse Mount { get; set; } = new();
    
    [JsonProperty("upload")] 
    public UploadResponse Upload { get; set; } = new();
}