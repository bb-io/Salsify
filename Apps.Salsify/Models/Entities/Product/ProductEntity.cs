using Newtonsoft.Json;

namespace Apps.Salsify.Models.Entities.Product;

public class ProductEntity
{
    [JsonProperty("salsify:system_id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("salsify:created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonProperty("salsify:updated_at")]
    public DateTime UpdatedAt { get; set; }
}