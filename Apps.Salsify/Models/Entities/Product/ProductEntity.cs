using Apps.Salsify.Extensions;
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

    [JsonProperty("salsify:version")]
    public int Version { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JToken> RawValues { get; set; } = new();
    
    [JsonIgnore]
    public IEnumerable<KeyValuePair<string, JToken>> Values => RawValues.Where(x => !x.Key.StartsWith("salsify:"));
    
    public IReadOnlyDictionary<string, IReadOnlyList<string>> GetLocalizedValues(string propertyId)
    {
        if (!RawValues.TryGetValue(propertyId, out var token))
            return new Dictionary<string, IReadOnlyList<string>>();

        return token is JObject obj
            ? obj.Properties().ToDictionary(x => x.Name, x => x.Value.Flatten())
            : new Dictionary<string, IReadOnlyList<string>> { [string.Empty] = token.Flatten() };
    }

    public IReadOnlyList<string> GetValues(string propertyId)
    {
        return RawValues.TryGetValue(propertyId, out var token) ? token is JArray array
                ? array.Select(x => x.ToString()).ToList()
                : [token.ToString()] 
            : [];
    }

    public string? GetValue(string propertyId) => GetValues(propertyId).FirstOrDefault();
}