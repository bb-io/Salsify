using Newtonsoft.Json;

namespace Apps.Salsify.Models.Utility.GraphQl;

public class GraphQlResponse<T>
{
    [JsonProperty("data")] 
    public T? Data { get; set; }
    
    [JsonProperty("errors")] 
    public List<GraphQlError> Errors { get; set; } = [];
}