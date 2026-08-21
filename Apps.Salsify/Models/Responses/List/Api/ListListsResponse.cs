using Apps.Salsify.Models.Entities.List;
using Apps.Salsify.Models.Utility.Pagination;
using Newtonsoft.Json;

namespace Apps.Salsify.Models.Responses.List.Api;

public class ListListsResponse : PaginatedResponse<ListEntity>
{
    [JsonProperty("lists")]
    public override List<ListEntity> Items { get; set; } = [];
}