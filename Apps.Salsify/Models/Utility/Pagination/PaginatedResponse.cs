using Newtonsoft.Json;

namespace Apps.Salsify.Models.Utility.Pagination;

public abstract class PaginatedResponse<TItem>
{
    [JsonProperty("meta")] 
    public PaginationMeta? Meta { get; set; }
    
    public abstract List<TItem> Items { get; set; }
}