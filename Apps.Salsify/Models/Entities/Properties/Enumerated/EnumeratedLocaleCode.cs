using Newtonsoft.Json;

namespace Apps.Salsify.Models.Entities.Properties.Enumerated;

public class EnumeratedLocaleCode
{
    [JsonProperty("code")] 
    public string Code { get; set; } = string.Empty;
}