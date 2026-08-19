using Newtonsoft.Json;

namespace Apps.Salsify.Models.Entities.Properties.Enumerated;

public class EnumeratedValueEntity
{
    [JsonProperty("externalId")] 
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("id")] 
    public string SystemId { get; set; } = string.Empty;
    
    [JsonProperty("name")] 
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("names")] 
    public List<EnumeratedLocalizedName> Names { get; set; } = [];

    public string? GetName(string locale) => Names.FirstOrDefault(x => x.Locale.Code == locale)?.Value;
}