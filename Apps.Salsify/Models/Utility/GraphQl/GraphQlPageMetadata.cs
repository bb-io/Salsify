using Newtonsoft.Json;

namespace Apps.Salsify.Models.Utility.GraphQl;

public class GraphQlPageMetadata
{
    [JsonProperty("totalEntries")] 
    public int TotalEntries { get; set; }
    
    [JsonProperty("hasNext")] 
    public bool? HasNext { get; set; }
}