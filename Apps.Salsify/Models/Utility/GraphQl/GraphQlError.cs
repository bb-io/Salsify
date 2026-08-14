using Newtonsoft.Json;

namespace Apps.Salsify.Models.Utility.GraphQl;

public class GraphQlError
{
    [JsonProperty("message")] 
    public string Message { get; set; } = string.Empty;
}