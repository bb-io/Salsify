using Newtonsoft.Json;

namespace Apps.Salsify.Models.Utility.Pagination;

public class PaginationMeta
{
    [JsonProperty("current_page")]
    public int CurrentPage { get; set; }

    [JsonProperty("per_page")]
    public int PerPage { get; set; }
    
    [JsonProperty("cursor")]
    public string? Cursor { get; set; }
    
    [JsonProperty("total_entries")]
    public int TotalEntries { get; set; }
}