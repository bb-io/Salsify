using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Salsify.Models.Entities.Product;

public class ProductEntity
{
    [JsonProperty("salsify:id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("salsify:created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonProperty("salsify:updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JToken> RawValues { get; set; } = new();

    public IReadOnlyList<string> GetValues(string propertyId)
    {
        return RawValues.TryGetValue(propertyId, out var token) ? token is JArray array
                ? array.Select(x => x.ToString()).ToList()
                : [token.ToString()] 
            : [];
    }

    public string? GetValue(string propertyId) => GetValues(propertyId).FirstOrDefault();
}