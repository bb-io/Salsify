using Newtonsoft.Json;

namespace Apps.Salsify.Models.Utility.GraphQl;

public class GraphQlPage<TItem>
{
    [JsonProperty("entries")] 
    public List<TItem> Entries { get; set; } = [];
    
    [JsonProperty("pageMetadata")] 
    public GraphQlPageMetadata PageMetadata { get; set; } = new();
}