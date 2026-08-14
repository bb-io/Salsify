using Newtonsoft.Json;

namespace Apps.Salsify.Models.Entities.Properties.Enumerated;

public class EnumeratedLocalizedName
{
    [JsonProperty("value")] 
    public string Value { get; set; } = string.Empty;
    
    [JsonProperty("locale")] 
    public EnumeratedLocaleCode Locale { get; set; } = new();
}